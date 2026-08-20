using Falcon.FSATS.ResourceManagement.Application;
using Falcon.FSATS.ResourceManagement.Domain;
using Falcon.FSATS.ResourceManagement.Infrastructure;

var controller = new ResourceStrategyController();
_ = new ResourceCoordinationService(controller);

IFoundationResourceBindingPort foundation = new DisabledFoundationResourcePort();
_ = new FoundationResourceBindingService(foundation);

Console.WriteLine("Falcon FSATS APP-RSC Host: internal coordination core present; exact Foundation resource binding pending.");
