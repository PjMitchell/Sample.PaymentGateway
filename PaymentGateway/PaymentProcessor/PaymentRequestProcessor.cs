using System;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public interface IPaymentRequestProcessor
    {
        Task<PaymentRequestResult> SubmitPaymentRequest(MerchantPaymentRequest paymentRequest);
    }

    public class PaymentRequestProcessor : IPaymentRequestProcessor
    {
        private readonly IPaymentRequestValidator validator;
        private readonly IBankTransactionService bankTransactionService;
        private readonly IEventBus eventBus;

        public PaymentRequestProcessor(IPaymentRequestValidator validator, IBankTransactionService bankTransactionService, IEventBus eventBus)
        {
            this.validator = validator;
            this.bankTransactionService = bankTransactionService;
            this.eventBus = eventBus;
        }

        public async Task<PaymentRequestResult> SubmitPaymentRequest(MerchantPaymentRequest paymentRequest)
        {

            var validationResult = validator.ValidatePaymentRequest(paymentRequest);
            if (validationResult.HasErrors)
            {
                var failedResult = new FailedPaymentRequestResult(paymentRequest.TrackingId, PaymentErrorTypes.InvalidRequest, validationResult.Errors);
                await eventBus.PublishEventAsync(new MerchantPaymentRequestFailedToProcess(paymentRequest, failedResult));
                return failedResult;
            }
            var bankTransactionResult = await bankTransactionService.RequestPayment(paymentRequest);

            var result = await HandleBankTransactionResult(paymentRequest, bankTransactionResult);
            return result;
        }

        private Task<PaymentRequestResult> HandleBankTransactionResult(MerchantPaymentRequest paymentRequest, BankTransactionResult result)
        {
            return result.Status switch
            {
                BankTransactionResultStatus.Accepted => HandleAcceptedBankTransactionResult(paymentRequest, result),
                BankTransactionResultStatus.Rejected => HandleRejectedBankTransactionResult(paymentRequest, result, PaymentErrorTypes.RejectedByBank),
                BankTransactionResultStatus.NotFound => HandleRejectedBankTransactionResult(paymentRequest, result, PaymentErrorTypes.BankNotFound),
                _ => throw new InvalidOperationException($"Not expecting Enum, not {result.Status}")
            };
        }

        private async Task<PaymentRequestResult> HandleAcceptedBankTransactionResult(MerchantPaymentRequest paymentRequest, BankTransactionResult bankingTransactionResult)
        {
            var result = new SucessfulPaymentRequestResult(paymentRequest.TrackingId);
            await eventBus.PublishEventAsync(new MerchantPaymentRequestSuccessfullyProcessed(paymentRequest, bankingTransactionResult));
            return result;
        }

        private async Task<PaymentRequestResult> HandleRejectedBankTransactionResult(MerchantPaymentRequest paymentRequest, BankTransactionResult bankingTransactionResult, string errorType)
        {
            var result = new FailedPaymentRequestResult(paymentRequest.TrackingId, errorType, bankingTransactionResult.Errors);
            await eventBus.PublishEventAsync(new MerchantPaymentRequestFailedToProcess(paymentRequest, result));
            return result;
        }
    }





    public static class PaymentErrorTypes
    {
        public const string InvalidRequest = "invalid_request";
        public const string RejectedByBank = "bank_rejected";
        public const string BankNotFound = "bank_not_found";


    }
}
