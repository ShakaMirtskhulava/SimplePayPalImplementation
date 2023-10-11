public class PayPalService : IPayPalService
{
    private readonly string _clientId = "AfKluVTBQWxPSrtjTAPNML-CF_R8XJ4RCPeaO6BQhhl6-LiDjNRfnH3Zj75E4UJya-yOa-uK2ifmweIE";
    private readonly string _clientSecret = "EMG2YvZTEpgRMy8a5UyOOPuXNrF1QY-5z9b4c5gDPNTfPRJbaPtrfXd48jCmIRUYvKij6HA8mrySD1Zn";
    private readonly Dictionary<string, string> _config;


    public PayPalService()
    {
        _config = new Dictionary<string, string>
        {
            {"mode", "sandbox" },
            {"clientId", _clientId },
            { "clientSecret", _clientSecret }
        };

    }

    public PayPal.Api.Payment CreateOrder(decimal amount, string returnUrl, string cancelUrl)
    {
        var oAuthTokenCredentials = new OAuthTokenCredential(_clientId, _clientSecret, _config);
        var accessToken = oAuthTokenCredentials.GetAccessToken();
        var apiContext = new APIContext(accessToken);

        var items = new List<Item>
        {
            new Item
            {
                name = "Membership Fee",
                currency = "USD",
                price = amount.ToString("0.00").Replace(',','.'),
                quantity = "1",
                sku = "membership"
            }
        };
        var itemsList = new ItemList()
        {
            items = items
        };

        var transaction = new Transaction
        {
            amount = new Amount
            {
                currency = "USD",
                total = amount.ToString("0.00").Replace(',','.'),
                details = new Details
                {
                    tax = "0",
                    shipping = "0",
                    subtotal = amount.ToString("0.00").Replace(',', '.')
                }
            },
            item_list = itemsList,
            description = "Membership Fee"
        };
        var transactions = new List<Transaction>
        {
            transaction
        };

        var payer = new Payer
        {
            payment_method = "paypal"
        };
        var payment = new PayPal.Api.Payment
        {
            intent = "sale",
            payer = payer,
            transactions = transactions,
            redirect_urls = new RedirectUrls
            {
                cancel_url = cancelUrl,
                return_url = returnUrl
            }
        };

        var createdPayment = payment.Create(apiContext);

        return createdPayment;
    }

    public PayPal.Api.Payment ExecutePayment(string paymentId, string payerId)
    {
        var oAuthTokenCredentials = new OAuthTokenCredential(_clientId, _clientSecret, _config);
        var accessToken = oAuthTokenCredentials.GetAccessToken();
        var apiContext = new APIContext(accessToken);

        var paymentExecution = new PaymentExecution
        {
            payer_id = payerId
        };

        var payment = new PayPal.Api.Payment
        {
            id = paymentId
        };

        var executedPayment = payment.Execute(apiContext, paymentExecution);

        return executedPayment;
    }
}
