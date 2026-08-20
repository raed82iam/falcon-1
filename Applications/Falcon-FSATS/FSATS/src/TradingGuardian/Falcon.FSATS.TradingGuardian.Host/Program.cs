using Falcon.FSATS.TradingGuardian.Application;
using Falcon.FSATS.TradingGuardian.Domain;
using Falcon.FSATS.TradingGuardian.Infrastructure;

var classifier = new IncidentClassifier();
var crisis = new CrisisStateMachine();
IGovernedProtectionCommandRoutePort route = new DisabledProtectionCommandPort();
var dispatcher = new GovernedProtectionCommandDispatcher(route);
_ = new ProtectionCoordinator(classifier, crisis, dispatcher);
Console.WriteLine("Falcon FSATS Trading Guardian Host: governed deterministic protection present; runtime command route not authorized.");
