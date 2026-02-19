using Autofac;
using Selerant.DevEx.Infrastructure.CommandResolver.Base;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.CommandResolver;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Wirings
{
    public class WiringCommandResolvers
    {
        public void Wiring(ContainerBuilder builder)
        {
            CommandResolverProvider.RegisterCommandResolver<DxAssessmentCommandResolver, DxAssessment>(builder);
            CommandResolverProvider.RegisterCommandResolver<DxAssessmentCollectionCommandResolver, DxAssessmentCollection>(builder);
        }
    }
}