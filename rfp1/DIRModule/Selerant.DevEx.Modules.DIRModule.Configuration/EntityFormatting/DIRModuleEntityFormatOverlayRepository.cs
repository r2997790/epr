using Selerant.DevEx.Configuration.BusinessLayer.Formatting;
using Selerant.DevEx.Configuration.Formatting;
using Selerant.DevEx.Configuration.Formatting.Helpers;
using Selerant.DevEx.Configuration.Infrastructure;
using System;
using System.Collections.Generic;
using Selerant.DevEx.Configuration.DTOs;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.EntityFormatting
{/// <summary>
 /// Represents the overlay configuration for entity formatting
 /// </summary>
    internal class DIRModuleEntityFormatOverlayRepository : BaseConfigurationRepository<EntityFormattingConfigurationOverlay>, IEntityFormatOverlayModuleRepository
    {

        private EntityFormattingConfigurationOverlay Version_3_9_0(EntityFormattingConfigurationOverlay config)
        {
            var newEntityFormat = new EntityFormat()
                    .WithTypename("DxAssessment");
            newEntityFormat.AddStandardAction(
                        new FormattingStandardAction().AddFilterWithTargetFormat(Filter.Default(), "DEFAULT").WithActionName("StandardAction").WithActionFormattingType("ToShortDescription")
                        );
            newEntityFormat.AddStandardAction(
                        new FormattingStandardAction().AddFilterWithTargetFormat(Filter.Default(), "ListControl").WithActionName("ListControl").WithActionFormattingType("ToShortDescription")
                        );
            newEntityFormat.AddMacroAction(
                        new FormattingMacroAction().AddFilterWithTargetFormat(Filter.Default(), "NavigatorHeaderCaption", "UdfDialogHeaderCaption").WithActionName("HeaderCaption").WithOutputFormat("{0} {1} {2} {3}")
                            .AddPlan(
                                new MacroPlan().WithIndex(0).WithValue("{DxMacroUtilities.GetLocaleText(DIR_AssessmentManager, AssessmentLbl)}")
                                )
                            .AddPlan(
                                new MacroPlan().WithIndex(1).WithValue("{DxAssessment.Code}")
                                )
                            .AddPlan(
                                new MacroPlan().WithIndex(2).WithValue("{DxMacroUtilities.GetLocaleText(DIR_AssessmentManager, AssessmentOfTypeLbl}")
                                )
                            .AddPlan(
                                new MacroPlan().WithIndex(3).WithValue("{DxAssessment.TypeCode}")
                                )
                        );
            newEntityFormat.AddMacroAction(
                        new FormattingMacroAction().AddFilterWithTargetFormat(Filter.Default(), "NavigatorHeaderSubCaption", "UdfDialogHeaderSubCaption").WithActionName("HeaderSubCaption").WithOutputFormat("{0} ({1})")
                            .AddPlan(
                                new MacroPlan().WithIndex(0).WithValue("{DxAssessment.Description}")
                                )
                            .AddPlan(
                                new MacroPlan().WithIndex(1).WithValue("{DxAssessment.Status}")
                                )
                        );
            newEntityFormat.AddMacroAction(
                        new FormattingMacroAction().WithActionName("ObjExp").WithOutputFormat("{0} {1}")
                            .AddPlan(
                                new MacroPlan().WithIndex(0).WithValue("{DxAssessment.Description}")
                                )
                            .AddPlan(
                                new MacroPlan().WithIndex(1).WithValue("{DxAssessment.Status}")
                                )
                        );
            newEntityFormat.AddMacroAction(
                new FormattingMacroAction().WithActionName("HeaderBreadCrumb").WithValue("{DxAssessment.Description}({DxAssessment.Code})"));

            config.EntityFormatChanges.Add(EntityFormatChange.Add(newEntityFormat));

            return config;
        }

        protected override IEnumerable<(Version Version, Func<EntityFormattingConfigurationOverlay, EntityFormattingConfigurationOverlay>)> ProvideConfigurationVersionHandlers()
        {
            return new List<(Version Version, Func<EntityFormattingConfigurationOverlay, EntityFormattingConfigurationOverlay>)>
            {
                (new Version(3, 9, 0), Version_3_9_0)
            };
        }
    }
}
