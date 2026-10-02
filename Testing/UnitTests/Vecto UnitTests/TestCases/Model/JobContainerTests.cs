using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.UnitTests.TestCases.Model;

public class JobContainerTests
{
    private static readonly TimeSpan BoundedWaitTimeout = TimeSpan.FromSeconds(10);

    private static JobContainer CreateJobContainer()
    {
        return new JobContainer(Mock.Of<ISumData>());
    }

    private static Mock<IVectoRun> CreateSuccessfulRunMock(int runIdentifier)
    {
        var run = new Mock<IVectoRun>();
        run.SetupGet(r => r.RunIdentifier).Returns(runIdentifier);
        run.SetupGet(r => r.JobRunIdentifier).Returns(runIdentifier);
        run.SetupGet(r => r.RunName).Returns($"run-{runIdentifier}");
        run.SetupGet(r => r.CycleName).Returns("cycle");
        run.SetupGet(r => r.RunSuffix).Returns(string.Empty);
        run.SetupGet(r => r.FinishedWithoutErrors).Returns(true);
        run.Setup(r => r.Run());
        return run;
    }

    private static Mock<ISimulatorFactory> CreateFactoryMock(IEnumerable<IVectoRun> runs, ISimulatorFactory followUpFactory = null)
    {
        var factory = new Mock<ISimulatorFactory>();
        factory.SetupProperty(f => f.SumData);
        factory.SetupProperty(f => f.JobNumber);
        factory.Setup(f => f.SimulationRuns()).Returns(runs);
        factory.Setup(f => f.FollowUpSimulatorFactory(It.IsAny<IDictionary<int, JobContainer.ProgressEntry>>()))
            .Returns(followUpFactory);
        return factory;
    }

    private static bool WaitUntil(Func<bool> condition, TimeSpan timeout)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout) {
            if (condition()) {
                return true;
            }
            Thread.Sleep(20);
        }
        return condition();
    }

    [Test]
    public void AddRuns_FollowUpFactoryThrowsPartwayThroughEnumeration_AllCompletedBecomesTrue()
    {
        var primaryRun = CreateSuccessfulRunMock(1);

        IEnumerable<IVectoRun> ThrowingFollowUpRuns()
        {
            yield return CreateSuccessfulRunMock(2).Object;
            throw new VectoException("synthetic follow-up read failure", ErrorCodes.VEHICLE_TOTAL_MASS_EXCEEDS_GROSS_VEHICLE_MASS);
        }

        var followUpFactory = new Mock<ISimulatorFactory>();
        followUpFactory.SetupProperty(f => f.SumData);
        followUpFactory.SetupProperty(f => f.JobNumber);
        followUpFactory.Setup(f => f.SimulationRuns()).Returns(ThrowingFollowUpRuns());

        var primaryFactory = CreateFactoryMock(new[] { primaryRun.Object }, followUpFactory.Object);

        var jobContainer = CreateJobContainer();
        jobContainer.AddRuns(primaryFactory.Object);
        jobContainer.Execute();

        var completed = WaitUntil(() => jobContainer.AllCompleted, BoundedWaitTimeout);

        Assert.Multiple(() =>
        {
            Assert.That(completed, Is.True, "AllCompleted should become true instead of stalling forever");
            Assert.That(jobContainer.HasFatalError, Is.True);
            Assert.That(jobContainer.GetProgress().Values.All(p => p.Done), Is.True,
                "every registered run must reach a terminal (Done) state");
        });
    }

    [Test]
    public void AddRuns_FactoryThrowsPartwayThroughEnumeration_NoPartialRunsRegistered()
    {
        IEnumerable<IVectoRun> ThrowingRuns()
        {
            yield return CreateSuccessfulRunMock(1).Object;
            yield return CreateSuccessfulRunMock(2).Object;
            throw new VectoException("synthetic read failure", ErrorCodes.VEHICLE_TOTAL_MASS_EXCEEDS_GROSS_VEHICLE_MASS);
        }

        var factory = new Mock<ISimulatorFactory>();
        factory.SetupProperty(f => f.SumData);
        factory.SetupProperty(f => f.JobNumber);
        factory.Setup(f => f.SimulationRuns()).Returns(ThrowingRuns());

        var jobContainer = CreateJobContainer();

        Assert.Throws<VectoException>(() => jobContainer.AddRuns(factory.Object));
        Assert.That(jobContainer.GetProgress(), Is.Empty,
            "no run yielded before the throw should have been published to the job container");
    }

    [Test]
    public void JobCompleted_FollowUpFactoryCreatorThrows_PrimaryRunStillReachesDoneAndAllCompleted()
    {
        var primaryRun = CreateSuccessfulRunMock(1);

        var primaryFactory = new Mock<ISimulatorFactory>();
        primaryFactory.SetupProperty(f => f.SumData);
        primaryFactory.SetupProperty(f => f.JobNumber);
        primaryFactory.Setup(f => f.SimulationRuns()).Returns(new[] { primaryRun.Object });
        primaryFactory
            .Setup(f => f.FollowUpSimulatorFactory(It.IsAny<IDictionary<int, JobContainer.ProgressEntry>>()))
            .Throws(new VectoException("synthetic follow-up factory creation failure", ErrorCodes.SIMFACTORY_FOLLOWUP_PRIMARY_FAILED));

        var jobContainer = CreateJobContainer();
        jobContainer.AddRuns(primaryFactory.Object);
        jobContainer.Execute();

        var completed = WaitUntil(() => jobContainer.AllCompleted, BoundedWaitTimeout);

        Assert.Multiple(() =>
        {
            Assert.That(completed, Is.True, "AllCompleted should become true instead of stalling forever");
            Assert.That(jobContainer.HasFatalError, Is.True);
        });
    }
}
