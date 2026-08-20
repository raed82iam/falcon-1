using Falcon.FSATS.FSTSimA.Application;
using Falcon.FSATS.FSTSimA.Domain;
using Falcon.FSATS.FSTSimA.Infrastructure;

var generator = new SyntheticMarketGenerator();
var assessor = new ValidationAssessor();
var evidence = new InMemorySimulationEvidenceSink();
_ = new SimulationCoordinator(generator, assessor, evidence);
Console.WriteLine("Falcon FSATS FSTSimA Host: deterministic non-Live simulation present; operational egress forbidden.");
