public class HomeController : Controller
{
    private readonly IPayPalService _payPalService;


    public HomeController(IPayPalService payPalService)
    {
        _payPalService = payPalService;
    }


    public IActionResult Index()
    {
        decimal amount = 100m;
        var returnUrl = Url.Action("Success", "Home", null, Request.Scheme)!;
        var cancelUrl = Url.Action("Cancel", "Home", null, Request.Scheme)!;

        var createdPayment = _payPalService.CreateOrder(amount, returnUrl, cancelUrl);

        string? approvalUrl = createdPayment.links
                                .Where(x => x.rel.ToLower() == "approval_url")
                                .Select(x => x.href)
                                .FirstOrDefault();

        if (approvalUrl == null)
            return NotFound("Couldn't found approval url");

        return Redirect(approvalUrl);
    }

    public IActionResult Success(string paymentId, string PayerID)
    {
        var executedPayment = _payPalService.ExecutePayment(paymentId, PayerID);
        if (executedPayment == null)
            return NotFound("Couldn't execute payment");

        //Retrive the payment's transactions for details
        var transactions = executedPayment.transactions;
        var currency = transactions[0].amount.currency;
        var subtotal = transactions[0].amount.details.subtotal;
        var tax = transactions[0].amount.details.tax;
        var description = transactions[0].description;
        var email = executedPayment.payer.payer_info.email;
        var merchant_id = executedPayment.payer.payer_info.payer_id;

        return View(transactions);
    }

    public IActionResult Cancel()
    {
        //Deal with cancelled payment
        return View();
    }

}
