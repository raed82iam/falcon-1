using Falcon.FSATS.FSAPMA.Application;
using Falcon.FSATS.FSAPMA.Domain;
using Falcon.FSATS.FSAPMA.Infrastructure;

var controller = new ProviderController();
var quota = new QuotaLedger();
IProviderEgressPort egress = new DisabledProviderEgressPort();
_ = new ProviderDataCoordinator(controller, quota, egress);
Console.WriteLine("Falcon FSATS FSAPMA Host: deterministic fabric present; provider egress not authorized.");
