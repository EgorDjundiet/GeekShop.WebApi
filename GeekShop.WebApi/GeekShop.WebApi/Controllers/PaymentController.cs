﻿using GeekShop.Domain;
using GeekShop.Domain.ViewModels;
using GeekShop.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeekShop.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]  
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<Payment>> GetAll()
        {
            return await _paymentService.GetAll();
        }

        [HttpGet("Get")]
        public async Task<Payment> Get(int id)
        {
            return await _paymentService.Get(id);
        }
       
        [HttpPost("GetByIds")]
        public async Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids)
        {
            return await _paymentService.GetByIds(ids);
        }

        [HttpPost("Add")]
        public async Task Add([FromBody]SubmitPaymentIn payment)
        {
            await _paymentService.Add(payment);
        }
      
        [HttpPut("Update")]
        public async Task Update(int id, [FromBody]SubmitPaymentIn payment) // Change model to Payment
        {
            await _paymentService.Update(id, payment);
        }

        [HttpDelete("Delete")]
        public async Task Delete(int id)
        {
            await _paymentService.Delete(id);
        }
    }
}