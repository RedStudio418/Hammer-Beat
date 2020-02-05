﻿using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace Melanchall.DryWetMidi.Composing
{
    internal sealed class SetProgramNumberAction : IPatternAction
    {
        #region Constructor

        public SetProgramNumberAction(SevenBitNumber programNumber)
        {
            ProgramNumber = programNumber;
        }

        #endregion

        #region Properties

        public SevenBitNumber ProgramNumber { get; }

        #endregion

        #region IPatternAction

        public PatternActionResult Invoke(long time, PatternContext context)
        {
            var programChangeEvent = new ProgramChangeEvent(ProgramNumber) { Channel = context.Channel };
            var timedEvent = new TimedEvent(programChangeEvent, time);

            return new PatternActionResult(time, new[] { timedEvent });
        }

        #endregion
    }
}
