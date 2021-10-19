using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth0Demo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policies.OrdersRead)]
        public List<OrderModel> GetOrders()
        {
            return new List<OrderModel>();
        }

        [HttpGet]
        [Authorize(Policies.OrdersFull)]
        public OrderModel CreateOrders()
        {
            return new OrderModel
            {
                Id = Guid.NewGuid().ToString("D"),
                Name = "New order"
            };
        }
    }
}