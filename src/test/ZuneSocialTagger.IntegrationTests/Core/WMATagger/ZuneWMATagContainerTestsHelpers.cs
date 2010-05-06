using ZuneSocialTagger.Core.IO.WMATagger;

namespace ZuneSocialTagger.IntegrationTests.Core.WMATagger
{
    public static class ZuneWMATagContainerTestsHelpers
    {
        public static ZuneWMATagContainer CreateEmptyContainer()
        {
            return new ZuneWMATagContainer(ASFTag.ASFTagFactory.CreateASFTagContainer());
        }
    }
}