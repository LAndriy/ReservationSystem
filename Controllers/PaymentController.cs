using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Models;
using Stripe;
using Stripe.Checkout;

namespace ReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Inicjalizacja sesji płatności
        [HttpPost("create-payment-intent")]
        public IActionResult CreatePaymentIntent([FromBody] PaymentIntentCreateRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
            {
                return BadRequest("Invalid payment request.");
            }

            var paymentIntentService = new PaymentIntentService();
            long amount = CalculateOrderAmount(request.Items);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "pln",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var paymentIntent = paymentIntentService.Create(options);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }

        private long CalculateOrderAmount(Item[] items)
        {
            return items.Sum(item => item.Amount);
        }

        [HttpPost("webhook")]
        public IActionResult HandleWebhook()
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    "your-webhook-signing-secret"
                );

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Webhook Error: {ex.Message}");
            }
        }


    }
}
