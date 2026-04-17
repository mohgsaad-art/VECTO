using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface ITorqueSplitter : IUpdateable, IPowerTrainComponent
    {
        List<double> TorqueRatio { get; set; }

        List<IResponse> PerformRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun);
    }
}
