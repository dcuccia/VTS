﻿using Vts.Common;

namespace Vts.MonteCarlo
{
    public interface IPhaseFunction
    {
        /// <summary>
        /// Method to scatter photon based on phase-function specific information
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        void ScatterToNewDirection(Direction incomingDirectionToModify);
    }
}