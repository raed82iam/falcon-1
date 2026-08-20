using Falcon.FSATS.Trading.Application;
using Falcon.FSATS.Trading.Domain;
using Falcon.FSATS.Trading.Infrastructure;

var reservations = new CapitalReservationLedger();
var executionQueue = new AccountScopedExecutionQueue();
IBrokerExecutionPort broker = new DisabledBrokerExecutionPort();
_ = new TradingDecisionPipeline(reservations);
_ = new ExecutionCoordinator(broker, executionQueue);
Console.WriteLine("Falcon FSATS Trading Host: implementation present; runtime/broker egress not authorized.");
