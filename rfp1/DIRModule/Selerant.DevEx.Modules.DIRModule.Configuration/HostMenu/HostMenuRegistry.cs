using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.Configuration.HostMenu
{
	public class DIRHostMenuRegistry : IHostMenuRegistry
	{
		public const string SEARCH_ASSESSMENT_ID = DIRModuleInfo.MODULE_NAME + "_Assessment_Search";
		public const string CREATE_ASSESSMENT_ID = DIRModuleInfo.MODULE_NAME + "_CreateAssessment";

		public void RegisterButtons(HostSettings hostSettings)
		{
			var directGroup = new HostSettings.ButtonsGroup()
			{
				Text = Locale.GetString(ResourceFiles.AssessmentManager, "HostMenuMain_Direct"),
				Icon = "DxDirect",
				CssStyle = "direct"
			};

            directGroup.Buttons.Add(new HostSettings.Button
            {
                ComponentIdentifiableString = $"Search|{SEARCH_ASSESSMENT_ID}",
                TextLocalized =  Locale.GetString(ResourceFiles.AssessmentManager, "DIR_NavAssessmentType_SearchPanel"),
                Icon = "DxSearchDirect"
            });

            directGroup.Buttons.Add(new HostSettings.Button
			{
				ComponentIdentifiableString = $"Create|{CREATE_ASSESSMENT_ID}",
				TextLocalized = Locale.GetString(ResourceFiles.AssessmentManager, "DIR_NewAssessment"),
				DialogHeight = 650,
				DialogWidth = 1050,
				OpenMode = "Dialog",
				DialogCaptionLocalized = Locale.GetString(ResourceFiles.AssessmentManager, "DIR_CreateAssessmentWiazrd"),
				Icon = "DxAddDirect"
			});

			hostSettings.ButtonsGroups.Add(directGroup);
		}
	}
}