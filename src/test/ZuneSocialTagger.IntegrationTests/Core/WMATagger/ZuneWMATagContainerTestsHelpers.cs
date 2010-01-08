using ZuneSocialTagger.Core.WMATagger;

namespace ZuneSocialTagger.IntegrationTests.Core.WMATagger
{
    public static class ZuneWMATagContainerTestsHelpers
    {
        public static ZuneWMATagContainer CreateEmptyContainer()
        {
            return new ZuneWMATagContainer(ASFTag.Net.ASFTagFactory.CreateASFTagContainer());
        }
    }
}