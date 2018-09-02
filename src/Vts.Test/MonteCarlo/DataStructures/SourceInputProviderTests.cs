using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SourceInputProviderTests
    {
        [Test]
        public void validate_GenerateAllSourceInputs_returns_non_empty_list_of_ISourceInputs()
        {
            var allSourceInputs = Vts.MonteCarlo.SourceInputProvider.GenerateAllSourceInputs();

            Assert.NotNull(allSourceInputs);
            Assert.True(allSourceInputs.Count > 0);
        }
    }
}
