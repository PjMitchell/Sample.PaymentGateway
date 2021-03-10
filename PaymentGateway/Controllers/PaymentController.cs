using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Auth;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PaymentGateway.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            this.logger = logger;
        }

        [Authorize(Policy = AuthPolicy.PaymentSubmission)]
        [ProducesResponseType(200, Type = typeof(SucessfulPaymentRequestResult))]
        [ProducesResponseType(400, Type = typeof(FailedPaymentRequestResult))]
        [HttpPost(Name = nameof(RequestPayment))]
        public async Task<IActionResult> RequestPayment([FromServices]IPaymentRequestProcessor paymentRequestProcessor, PaymentRequest request)
        {
            var sw = Stopwatch.StartNew();
            var merchantId = User.FindFirst(MerchantClaims.MechantIdClaimType)?.Value ?? string.Empty;
            var merchantRequest = new MerchantPaymentRequest(merchantId, request);
            var result = await paymentRequestProcessor.SubmitPaymentRequest(merchantRequest);
            ObjectResult actionResult = result switch
            {
                SucessfulPaymentRequestResult success => Ok(success),
                FailedPaymentRequestResult failed => BadRequest(failed),
                _ => throw new InvalidOperationException($"There should only be two types of response Sucess or Failed, not {result.GetType()}")
            };
            logger.LogInformation("Payment request ({TrackingId}) completed with status code:{StatusCode} in {ElapsedMilliseconds}ms", merchantRequest.TrackingId, actionResult.StatusCode, sw.ElapsedMilliseconds);
            return actionResult;

        }

        [Authorize(Policy = AuthPolicy.PaymentRead)]
        [ProducesResponseType(200, Type = typeof(CompletedMerchantPaymentRequestEvent))]  
        [ProducesResponseType(404)]
        [HttpGet("{trackingId}", Name = nameof(GetPaymentRequest))]
        public async Task<IActionResult> GetPaymentRequest([FromServices]IPaymentStore store, string trackingId)
        {
            var merchantId = User.FindFirst(MerchantClaims.MechantIdClaimType)?.Value ?? string.Empty;
            var result = await store.GetAsync(merchantId, trackingId);
            if (result is null)
                return NotFound();
            return Ok(result);
        }
    }
}
