using System;
using Selerant.ApplicationBlocks.Data.Migrations;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DX_PHRASE_DEF : MigrationBase
	{
		public override void Up()
		{
			#region DX_ATTRIBUTE_DEF.SCOPE

			Database.Data.InsertInto("DX_PHRASE_DEF")
					.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
					.Values(11, "DX_ATTRIBUTE_DEF.SCOPE", "ASSESSMENT", null, 0);

			#endregion

			#region DX_MATERIAL.MAT_TYPE

			Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(11, "DX_MATERIAL.MAT_TYPE", "DIR_RESOURCE", null, 0);

            #endregion

            #region DXDIR_ASSESSMENT.STATUS

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "DEVELOPMENT", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "SUBMITTED", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "APPROVED", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "CURRENT", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "HISTORIC", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
					.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.STATUS", "DRAFT", null, 0);

			#endregion

			#region DXDIR_ASSESSMENT.ORG_STRUCTURE

			Database.Data.InsertInto("DX_PHRASE_DEF")
					.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.ORG_STRUCTURE", "FACTORY", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
					.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.ORG_STRUCTURE", "FARM", null, 0);

			#endregion

			#region DXDIR_ASSESSMENT.PROD_CLASSIF

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01111", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01112", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01121", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01122", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01131", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01132", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01141", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01142", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01151", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01152", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01161", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01162", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01171", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01172", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01181", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01182", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01191", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01192", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01193", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01194", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01195", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01199", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01211", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01212", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01213", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01214", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01215", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01216", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01219", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01221", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01229", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01231", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01232", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01233", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01234", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01235", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01239", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01241", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01242", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01243", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01249", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01251", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01252", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01253", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01254", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01259", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01260", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01270", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01290", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01311", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01312", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01313", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01314", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01315", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01316", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01317", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01318", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01319", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01321", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01322", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01323", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01324", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01329", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01330", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01341", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01342", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01343", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01344", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01345", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01346", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01349", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01351", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01352", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01353", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01354", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01355", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01356", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01359", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01360", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01371", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01372", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01373", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01374", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01375", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01376", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01377", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01379", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01411", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01412", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01421", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01422", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01431", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01432", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01441", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01442", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01443", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01444", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01445", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01446", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01447", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01448", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01449", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01450", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01460", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01491", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01492", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01499", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01510", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01520", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01530", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01540", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01550", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01591", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01599", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01610", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01620", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01630", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01640", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01651", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01652", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01653", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01654", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01655", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01656", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01657", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01658", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01659", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01691", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01699", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01701", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01702", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01703", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01704", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01705", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01706", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01707", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01708", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01709", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01801", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01802", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01803", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01809", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01911", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01912", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01913", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01919", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01921", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01922", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01929", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01930", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01940", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01950", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01961", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01962", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01963", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01970", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01990", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02111", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02112", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02119", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02121", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02122", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02123", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02129", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02131", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02132", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02133", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02140", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02151", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02152", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02153", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02154", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02155", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02191", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02192", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02193", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02194", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02195", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02196", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02199", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02211", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02212", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02291", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02292", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02293", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02299", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02311", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02312", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02321", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02322", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02411", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02419", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02420", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02910", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02920", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02930", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02941", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02942", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02943", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02944", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02951", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02952", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02953", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02954", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02955", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02959", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02960", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04111", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04112", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04191", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04192", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04211", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04212", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04221", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04222", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04231", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04232", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04241", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04242", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04251", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04252", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04261", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04262", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04291", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04292", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04311", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04312", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04321", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04322", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04331", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04332", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04341", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04342", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04351", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04352", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04361", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04362", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04391", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04392", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04411", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04412", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04421", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04422", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04431", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04432", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04441", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04442", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04451", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04452", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04461", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04462", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04471", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04472", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04491", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04492", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04511", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04512", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04521", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04522", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04530", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04590", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04911", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04912", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04913", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04920", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04931", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04932", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04933", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04934", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21111", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21112", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21113", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21114", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21115", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21116", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21117", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21118", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21119", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21121", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21122", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21123", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21124", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21125", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21131", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21132", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21133", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21134", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21135", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21136", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21137", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21138", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21139", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21141", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21142", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21143", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21144", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21145", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21151", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21152", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21153", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21155", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21156", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21159", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21160", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21170", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21181", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21182", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21183", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21184", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21185", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21186", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21189", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21190", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21211", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21212", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21213", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21214", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21215", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21216", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21219", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21221", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21222", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21223", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21224", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21225", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21226", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21227", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21231", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21232", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21233", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21234", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21241", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21242", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21243", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21251", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21252", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21253", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21254", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21255", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21256", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21259", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21261", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21262", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21263", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21264", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21265", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21266", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21267", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21268", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21269", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21270", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21280", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21291", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21299", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21311", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21312", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21313", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21319", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21321", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21329", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21330", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21340", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21391", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21392", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21393", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21394", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21395", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21396", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21397", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21399", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21411", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21412", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21419", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21421", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21422", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21423", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21424", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21429", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21431", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21432", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21433", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21434", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21435", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21439", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21491", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21492", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21493", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21494", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21495", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21496", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21499", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21511", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21512", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21513", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21514", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21515", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21519", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21521", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21522", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21523", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21524", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21525", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21526", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21529", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21590", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21611", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21612", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21621", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21622", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21631", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21632", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21641", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21642", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21651", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21652", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21661", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21662", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21671", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21672", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21673", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21681", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21682", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21691", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21693", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21700", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21800", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21910", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21920", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21931", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21932", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22110", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22120", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22130", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22211", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22212", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22219", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22221", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22222", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22229", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22230", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22241", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22242", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22249", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22251", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22252", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22253", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22254", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22259", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22260", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22270", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22290", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22300", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23110", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23120", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23130", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23140", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23161", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23162", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23170", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23180", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23210", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23220", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23230", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23311", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23319", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23320", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23410", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23420", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23430", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23490", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23511", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23512", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23520", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23530", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23540", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23610", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23620", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23630", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23640", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23650", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23660", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23670", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23710", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23721", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23722", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23911", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23912", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23913", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23914", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23921", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23922", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23923", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23924", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23925", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23926", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23927", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23928", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23929", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23991", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23992", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23993", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23994", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23995", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23996", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23997", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23999", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24110", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24131", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24139", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24211", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24212", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24220", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24230", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24310", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24320", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24410", 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24490", 0);

            #endregion

            #region RESOURCE TYPE

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(10, "0", "99510", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(4, "99510", "FOOD", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                    .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                    .Values(4, "99510", "NONFOOD", null, 0);

            #endregion

            #region DXDIR_BUSINESS_COST.TITLE

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "ELECTRICITY", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "GAS", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "RENT", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "WAGES", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "MANAGEMENT", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "FACILITIES", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "STORAGE", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "FREIGHT", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "WASTE", null, 0);

            Database.Data.InsertInto("DX_PHRASE_DEF")
                .Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", "MATLOSS", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_BUSINESS_COST.TITLE", "OTHER", null, 0);

			#endregion

			#region DXDIR_RESULTS

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "UTILISED_INPUT", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "STOCK_HOLDING", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "FOOD_LOSS", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "FOOD_EDIBLE_LOSS_INPUTS", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "FOOD_EDIBLE_LOSS", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "LOSS_EX_WATER", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "PACK_LOSS", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "OUTPUT_PACK", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "ESTIM_COST", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "WASTE_REMOVAL", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "ENERGY_SPEND", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "MAT_SPEND", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "DISPOSAL", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "ESTIM_COST_WASTE", null, 0);

			#endregion DXDIR_RESULTS

			#region DXDIR_DATA_QUALITY

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(10, "0", "99511", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(4, "99511", "1", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(4, "99511", "2", null, 0);

			Database.Data.InsertInto("DX_PHRASE_DEF")
				.Columns("TYPE", "SUBTYPE", "CODE", "CALC_ID", "FLAGS")
				.Values(4, "99511", "3", null, 0);

			#endregion
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
