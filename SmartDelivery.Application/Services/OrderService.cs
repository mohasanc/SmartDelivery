using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Orders;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Application.Interfaces.Services;
using SmartDelivery.Domain.Entities;
using SmartDelivery.Domain.Enums;

namespace SmartDelivery.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
        {
            [OrderStatus.Pending] = new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
            [OrderStatus.Confirmed] = new[] { OrderStatus.Preparing, OrderStatus.Cancelled },
            [OrderStatus.Preparing] = new[] { OrderStatus.ReadyForPickup },
            [OrderStatus.ReadyForPickup] = new[] { OrderStatus.OnTheWay },
            [OrderStatus.OnTheWay] = new[] { OrderStatus.Delivered },
            [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
            [OrderStatus.Cancelled] = Array.Empty<OrderStatus>(),
        };

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> GetByIdAsync(Guid id, Guid requesterId, IEnumerable<string> roles, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.Orders.GetWithDetailsAsync(id, cancellationToken);
            if (order is null || order.IsDeleted)
                return Result<OrderDto>.NotFound("Order not found.");

            var roleList = roles.ToList();
            var isAdmin = roleList.Contains("Admin");
            var isDriver = roleList.Contains("Driver") && order.DriverId == requesterId;
            var isCustomer = order.CustomerId == requesterId;
            var isRestaurantOwner = roleList.Contains("RestaurantOwner") && order.Restaurant?.OwnerId == requesterId;

            if (!isAdmin && !isDriver && !isCustomer && !isRestaurantOwner)
                return Result<OrderDto>.Forbidden("You are not authorized to view this order.");

            var dto = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Success(dto);
        }
        public async Task<Result<PagedResult<OrderSummaryDto>>> GetAllAsync(OrderQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.Orders.GetPagedAsync(parameters, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<OrderSummaryDto>>(result.Items);

            return Result<PagedResult<OrderSummaryDto>>.Success(
                PagedResult<OrderSummaryDto>.Create(dtos, result.TotalCount, result.PageNumber, result.PageSize));
        }

        public async Task<Result<PagedResult<OrderSummaryDto>>> GetMyOrdersAsync(Guid customerId, OrderQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            parameters.CustomerId = customerId;
            var result = await _unitOfWork.Orders.GetPagedAsync(parameters, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<OrderSummaryDto>>(result.Items);

            return Result<PagedResult<OrderSummaryDto>>.Success(
                PagedResult<OrderSummaryDto>.Create(dtos, result.TotalCount, result.PageNumber, result.PageSize));
        }

        public async Task<Result<PagedResult<OrderSummaryDto>>> GetDriverOrdersAsync(Guid driverId, OrderQueryParameters parameters, CancellationToken cancellationToken = default)
        {
            parameters.DriverId = driverId;

            var result = await _unitOfWork.Orders.GetPagedAsync(parameters, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<OrderSummaryDto>>(result.Items);

            return Result<PagedResult<OrderSummaryDto>>.Success(
                PagedResult<OrderSummaryDto>.Create(dtos, result.TotalCount, result.PageNumber, result.PageSize));
        }
        public async Task<Result<OrderDto>> CreateAsync(Guid customerId, CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId, cancellationToken);
                if (restaurant is null || restaurant.IsDeleted || !restaurant.IsActive)
                    return Result<OrderDto>.NotFound("Restaurant not found or inactive.");

                if (!restaurant.IsOpen)
                    return Result<OrderDto>.Failure("Restaurant is currently closed.");

                // validate all menu items
                var orderItems = new List<OrderItem>();
                decimal subTotal = 0;

                foreach (var itemRequest in request.Items)
                {
                    var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(itemRequest.MenuItemId, cancellationToken);

                    if (menuItem is null || menuItem.IsDeleted || !menuItem.IsActive)
                        return Result<OrderDto>.Failure($"Menu item {itemRequest.MenuItemId} not found.");

                    if (!menuItem.IsAvailable)
                        return Result<OrderDto>.Failure($"'{menuItem.Name}' is currently unavailable.");

                    if (menuItem.RestaurantId != request.RestaurantId)
                        return Result<OrderDto>.Failure($"Menu item '{menuItem.Name}' does not belong to this restaurant.");

                    var itemTotal = menuItem.Price * itemRequest.Quantity;
                    subTotal += itemTotal;

                    orderItems.Add(new OrderItem
                    {
                        MenuItemId = menuItem.Id,
                        MenuItemName = menuItem.Name,
                        UnitPrice = menuItem.Price,
                        Quantity = itemRequest.Quantity,
                        TotalPrice = itemTotal,
                        SpecialRequests = itemRequest.SpecialRequests
                    });
                }
                const decimal taxRate = 0.14m; // 14% VAT
                var tax = Math.Round(subTotal * taxRate, 2);
                var total = subTotal + restaurant.DeliveryFee + tax;
                var orderNumber = await _unitOfWork.Orders.GenerateOrderNumberAsync(cancellationToken);

                var order = new Order
                {
                    OrderNumber = orderNumber,
                    CustomerId = customerId,
                    RestaurantId = request.RestaurantId,
                    Status = OrderStatus.Pending,
                    SubTotal = subTotal,
                    DeliveryFee = restaurant.DeliveryFee,
                    Tax = tax,
                    TotalAmount = total,
                    DeliveryAddress = request.DeliveryAddress,
                    DeliveryLatitude = request.DeliveryLatitude,
                    DeliveryLongitude = request.DeliveryLongitude,
                    SpecialInstructions = request.SpecialInstructions,
                    PaymentMethod = request.PaymentMethod,
                    EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(restaurant.EstimatedDeliveryMinutes),
                    OrderItems = orderItems
                };

                await _unitOfWork.Orders.AddAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Order {OrderNumber} created by customer {CustomerId}", order.OrderNumber, customerId);

                var created = await _unitOfWork.Orders.GetWithDetailsAsync(order.Id, cancellationToken);
                var dto = _mapper.Map<OrderDto>(created);

                return Result<OrderDto>.Created(dto);

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", customerId);
                return Result<OrderDto>.Failure("Order Creation failed", 500);
            }
        }
        public async Task<Result<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, Guid requesterId,
            IEnumerable<string> roles, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.Orders.GetWithDetailsAsync(id, cancellationToken);
            if (order is null || order.IsDeleted)
                return Result<OrderDto>.NotFound("Order not found.");

            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
                return Result<OrderDto>.Failure("Invalid order status");

            if (!AllowedTransitions[order.Status].Contains(newStatus))
                return Result<OrderDto>.Failure($"Cannot transition from {order.Status} to {newStatus}.");

            var roleList = roles.ToList();
            //Authorization Rules
            if (newStatus == OrderStatus.Cancelled)
            {
                if (order.CustomerId != requesterId && !roleList.Contains("Admin") && !roleList.Contains("RestaurantOwner"))
                    return Result<OrderDto>.Forbidden("Only the customer, restaurant owner or admin can cancel an order.");

                order.CancellationReason = request.CancellationReason;
            }

            if (newStatus == OrderStatus.Confirmed && order.Restaurant?.OwnerId != requesterId && !roleList.Contains("Admin"))
                return Result<OrderDto>.Forbidden("Only the restaurant owner or admin can confirm orders.");

            if (newStatus is OrderStatus.OnTheWay && order.DriverId != requesterId && !roleList.Contains("Admin"))
                return Result<OrderDto>.Forbidden("Only the assigned driver or admin can pick up the order.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            // Record timestamps for each status
            switch (newStatus)
            {
                case OrderStatus.Confirmed:
                    order.ConfirmedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Preparing:
                    order.PreparingAt = DateTime.UtcNow;
                    break;
                case OrderStatus.ReadyForPickup:
                    order.ReadyAt = DateTime.UtcNow;
                    break;
                case OrderStatus.OnTheWay:
                    order.PickedUpAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.ActualDeliveryTime = DateTime.UtcNow;
                    order.IsPaid = order.PaymentMethod != "Cash"; // Card/Wallet already paid
                    break;
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Order {OrderNumber} status changed to {Status}", order.OrderNumber, newStatus);

            var dto = _mapper.Map<OrderDto>(order);

            return Result<OrderDto>.Success(dto);
        }
        public async Task<Result<OrderDto>> AssignDriverAsync(Guid orderId, AssignDriverRequest request, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.Orders.GetWithDetailsAsync(orderId, cancellationToken);
            if (order is null || order.IsDeleted)
                return Result<OrderDto>.NotFound("Order not found.");

            if (order.Status != OrderStatus.ReadyForPickup)
                return Result<OrderDto>.Failure("Order must be in ReadyForPickup status to assign a driver.");

            var driver = await _unitOfWork.Users.GetByIdAsync(request.DriverId, cancellationToken);
            if (driver is null || !driver.IsActive)
                return Result<OrderDto>.NotFound("Driver not found.");

            var driverRoles = await _unitOfWork.Users.GetUserRolesAsync(request.DriverId, cancellationToken);
            if (!driverRoles.Contains("Driver"))
                return Result<OrderDto>.Failure("The specified user is not a driver.");

            order.DriverId = request.DriverId;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Driver {DriverId} assigned to order {OrderId}", request.DriverId, orderId);

            var updated = await _unitOfWork.Orders.GetWithDetailsAsync(orderId, cancellationToken);
            var dto = _mapper.Map<OrderDto>(updated!);

            return Result<OrderDto>.Success(dto);
        }

        public async Task<Result> RateOrderAsync(Guid orderId, Guid customerId, RateOrderRequest request, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);
            if (order is null || order.IsDeleted)
                return Result.NotFound("Order not found.");

            if (order.CustomerId != customerId)
                return Result.Failure("Unauthorized.", 403);

            if (order.Status != OrderStatus.Delivered)
                return Result.Failure("Can only rate delivered orders.");

            if (order.CustomerRating.HasValue)
                return Result.Failure("Order has already been rated.");

            order.CustomerRating = request.Rating;
            order.CustomerReview = request.Review;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);

            // Update restaurant rating
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(order.RestaurantId, cancellationToken);
            if (restaurant is not null)
            {
                var newTotal = restaurant.TotalRatings + 1;
                restaurant.Rating = ((restaurant.Rating * restaurant.TotalRatings) + request.Rating) / newTotal;

                restaurant.TotalRatings = newTotal;

                _unitOfWork.Restaurants.Update(restaurant);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success("Thank you for your rating!");
        }
    }
}
