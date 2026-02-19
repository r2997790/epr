using System;
using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
	public class DX_PHRASE_TEXT : MigrationBase
	{
		public override void Up()
		{
			#region DX_ATTRIBUTE_DEF.SCOPE

			Database.Data.InsertInto("DX_PHRASE_TEXT")
					.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
					.Values(11, "DX_ATTRIBUTE_DEF.SCOPE", "ASSESSMENT", "en", DbValue.Nvarchar("ASSESSMENT"), 0);

			#endregion

			#region DX_MATERIAL.MAT_TYPE

			Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DX_MATERIAL.MAT_TYPE", "DIR_RESOURCE", "en", DbValue.Nvarchar("DIRECT Resource"), 0);

            #endregion

            #region DXDIR_ASSESSMENT.STATUS

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "DEVELOPMENT", "en", DbValue.Nvarchar("Development"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "SUBMITTED", "en", DbValue.Nvarchar("Submitted"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "APPROVED", "en", DbValue.Nvarchar("Approved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "CURRENT", "en", DbValue.Nvarchar("Current"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.STATUS", "HISTORIC", "en", DbValue.Nvarchar("Historic"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
					.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.STATUS", "DRAFT", "en", DbValue.Nvarchar("Draft"), 0);

			#endregion

			#region DXDIR_ASSESSMENT.ORG_STRUCTURE

			Database.Data.InsertInto("DX_PHRASE_TEXT")
					.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.ORG_STRUCTURE", "FACTORY", "en", DbValue.Nvarchar("Factory"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
					.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.ORG_STRUCTURE", "FARM", "en", DbValue.Nvarchar("Farm"), 0);

			#endregion

			#region DXDIR_ASSESSMENT.PROD_CLASSIF

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01111", "en", DbValue.Nvarchar("Wheat, seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01112", "en", DbValue.Nvarchar("Wheat, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01121", "en", DbValue.Nvarchar("Maize (corn), seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01122", "en", DbValue.Nvarchar("Maize (corn), other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01131", "en", DbValue.Nvarchar("Rice, seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01132", "en", DbValue.Nvarchar("Rice paddy, other (not husked)"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01141", "en", DbValue.Nvarchar("Sorghum, seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01142", "en", DbValue.Nvarchar("Sorghum, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01151", "en", DbValue.Nvarchar("Barley, seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01152", "en", DbValue.Nvarchar("Barley, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01161", "en", DbValue.Nvarchar("Rye, seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01162", "en", DbValue.Nvarchar("Rye, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01171", "en", DbValue.Nvarchar("Oats, seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01172", "en", DbValue.Nvarchar("Oats, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01181", "en", DbValue.Nvarchar("Millet, seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01182", "en", DbValue.Nvarchar("Millet, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01191", "en", DbValue.Nvarchar("Triticale"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01192", "en", DbValue.Nvarchar("Buckwheat"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01193", "en", DbValue.Nvarchar("Fonio"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01194", "en", DbValue.Nvarchar("Quinoa"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01195", "en", DbValue.Nvarchar("Canary seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01199", "en", DbValue.Nvarchar("Other cereals n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01211", "en", DbValue.Nvarchar("Asparagus"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01212", "en", DbValue.Nvarchar("Cabbages"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01213", "en", DbValue.Nvarchar("Cauliflowers and broccoli"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01214", "en", DbValue.Nvarchar("Lettuce and chicory"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01215", "en", DbValue.Nvarchar("Spinach"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01216", "en", DbValue.Nvarchar("Artichokes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01219", "en", DbValue.Nvarchar("Other leafy or stem vegetables"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01221", "en", DbValue.Nvarchar("Watermelons"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01229", "en", DbValue.Nvarchar("Cantaloupes and other melons"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01231", "en", DbValue.Nvarchar("Chillies and peppers, green (<i>Capsicum</i> spp. and <i>Pimenta</i> spp.)"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01232", "en", DbValue.Nvarchar("Cucumbers and gherkins"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01233", "en", DbValue.Nvarchar("Eggplants (aubergines)"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01234", "en", DbValue.Nvarchar("Tomatoes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01235", "en", DbValue.Nvarchar("Pumpkins, squash and gourds"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01239", "en", DbValue.Nvarchar("Other fruit-bearing vegetables"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01241", "en", DbValue.Nvarchar("Beans, green"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01242", "en", DbValue.Nvarchar("Peas, green"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01243", "en", DbValue.Nvarchar("Broad beans and horse beans, green"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01249", "en", DbValue.Nvarchar("Other green leguminous vegetables"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01251", "en", DbValue.Nvarchar("Carrots and turnips"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01252", "en", DbValue.Nvarchar("Green garlic"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01253", "en", DbValue.Nvarchar("Onions"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01254", "en", DbValue.Nvarchar("Leeks and other alliaceous vegetables"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01259", "en", DbValue.Nvarchar("Other root, bulb and tuberous vegetables, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01260", "en", DbValue.Nvarchar("Vegetable seeds, except beet seeds"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01270", "en", DbValue.Nvarchar("Mushrooms and truffles"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01290", "en", DbValue.Nvarchar("Vegetables, fresh, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01311", "en", DbValue.Nvarchar("Avocados"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01312", "en", DbValue.Nvarchar("Bananas"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01313", "en", DbValue.Nvarchar("Plantains and cooking bananas"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01314", "en", DbValue.Nvarchar("Dates"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01315", "en", DbValue.Nvarchar("Figs"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01316", "en", DbValue.Nvarchar("Mangoes, guavas and mangosteens"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01317", "en", DbValue.Nvarchar("Papayas"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01318", "en", DbValue.Nvarchar("Pineapples"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01319", "en", DbValue.Nvarchar("Other tropical and subtropical fruits, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01321", "en", DbValue.Nvarchar("Pomelos and grapefruits"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01322", "en", DbValue.Nvarchar("Lemons and limes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01323", "en", DbValue.Nvarchar("Oranges"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01324", "en", DbValue.Nvarchar("Tangerines, mandarins, clementines"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01329", "en", DbValue.Nvarchar("Other citrus fruit, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01330", "en", DbValue.Nvarchar("Grapes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01341", "en", DbValue.Nvarchar("Apples"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01342", "en", DbValue.Nvarchar("Pears and quinces"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01343", "en", DbValue.Nvarchar("Apricots"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01344", "en", DbValue.Nvarchar("Cherries"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01345", "en", DbValue.Nvarchar("Peaches and nectarines"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01346", "en", DbValue.Nvarchar("Plums and sloes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01349", "en", DbValue.Nvarchar("Other pome fruits and stone fruits"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01351", "en", DbValue.Nvarchar("Currants and gooseberries"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01352", "en", DbValue.Nvarchar("Kiwi fruit"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01353", "en", DbValue.Nvarchar("Raspberries, blackberries, mulberries and loganberries"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01354", "en", DbValue.Nvarchar("Strawberries"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01355", "en", DbValue.Nvarchar("Other berries; fruits of the genus <i>Vaccinium</i>"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01356", "en", DbValue.Nvarchar("Locust beans (carobs)"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01359", "en", DbValue.Nvarchar("Other fruits, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01360", "en", DbValue.Nvarchar("Fruit seeds"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01371", "en", DbValue.Nvarchar("Almonds, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01372", "en", DbValue.Nvarchar("Cashew nuts, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01373", "en", DbValue.Nvarchar("Chestnuts, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01374", "en", DbValue.Nvarchar("Hazelnuts, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01375", "en", DbValue.Nvarchar("Pistachios, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01376", "en", DbValue.Nvarchar("Walnuts, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01377", "en", DbValue.Nvarchar("Brazil nuts, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01379", "en", DbValue.Nvarchar("Other nuts (excluding wild edible nuts and groundnuts), in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01411", "en", DbValue.Nvarchar("Soya beans, seed for planting"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01412", "en", DbValue.Nvarchar("Soya beans, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01421", "en", DbValue.Nvarchar("Groundnuts, seed for planting"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01422", "en", DbValue.Nvarchar("Groundnuts, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01431", "en", DbValue.Nvarchar("Cottonseed, seed for planting"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01432", "en", DbValue.Nvarchar("Cottonseed, other"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01441", "en", DbValue.Nvarchar("Linseed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01442", "en", DbValue.Nvarchar("Mustard seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01443", "en", DbValue.Nvarchar("Rape or colza seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01444", "en", DbValue.Nvarchar("Sesame seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01445", "en", DbValue.Nvarchar("Sunflower seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01446", "en", DbValue.Nvarchar("Safflower seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01447", "en", DbValue.Nvarchar("Castor oil seeds"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01448", "en", DbValue.Nvarchar("Poppy seed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01449", "en", DbValue.Nvarchar("Other oilseeds, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01450", "en", DbValue.Nvarchar("Olives"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01460", "en", DbValue.Nvarchar("Coconuts, in shell"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01491", "en", DbValue.Nvarchar("Palm nuts and kernels"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01492", "en", DbValue.Nvarchar("Copra"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01499", "en", DbValue.Nvarchar("Other oleaginous fruits, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01510", "en", DbValue.Nvarchar("Potatoes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01520", "en", DbValue.Nvarchar("Cassava"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01530", "en", DbValue.Nvarchar("Sweet potatoes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01540", "en", DbValue.Nvarchar("Yams"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01550", "en", DbValue.Nvarchar("Taro"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01591", "en", DbValue.Nvarchar("Yautia"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01599", "en", DbValue.Nvarchar("Other edible roots and tubers with high starch or inulin content, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01610", "en", DbValue.Nvarchar("Coffee, green"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01620", "en", DbValue.Nvarchar("Tea leaves"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01630", "en", DbValue.Nvarchar("MatÃ© leaves"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01640", "en", DbValue.Nvarchar("Cocoa beans"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01651", "en", DbValue.Nvarchar("Pepper (<i>Piper</i> spp.), raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01652", "en", DbValue.Nvarchar("Chillies and peppers, dry (<i>Capsicum</i> spp., <i>Pimenta</i> spp.), raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01653", "en", DbValue.Nvarchar("Nutmeg, mace, cardamoms, raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01654", "en", DbValue.Nvarchar("Anise, badian, coriander, cumin, caraway, fennel and juniper berries, raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01655", "en", DbValue.Nvarchar("Cinnamon and cinnamon-tree flowers, raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01656", "en", DbValue.Nvarchar("Cloves (whole stems), raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01657", "en", DbValue.Nvarchar("Ginger, raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01658", "en", DbValue.Nvarchar("Vanilla, raw"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01659", "en", DbValue.Nvarchar("Hop cones"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01691", "en", DbValue.Nvarchar("Chicory roots"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01699", "en", DbValue.Nvarchar("Other stimulant, spice and aromatic crops, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01701", "en", DbValue.Nvarchar("Beans, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01702", "en", DbValue.Nvarchar("Broad beans and horse beans, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01703", "en", DbValue.Nvarchar("Chick peas, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01704", "en", DbValue.Nvarchar("Lentils, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01705", "en", DbValue.Nvarchar("Peas, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01706", "en", DbValue.Nvarchar("Cow peas, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01707", "en", DbValue.Nvarchar("Pigeon peas, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01708", "en", DbValue.Nvarchar("Bambara beans, dry"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01709", "en", DbValue.Nvarchar("Pulses, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01801", "en", DbValue.Nvarchar("Sugar beet"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01802", "en", DbValue.Nvarchar("Sugar cane"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01803", "en", DbValue.Nvarchar("Sugar beet seeds"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01809", "en", DbValue.Nvarchar("Other sugar crops n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01911", "en", DbValue.Nvarchar("Maize for forage and silage"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01912", "en", DbValue.Nvarchar("Alfalfa for forage and silage"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01913", "en", DbValue.Nvarchar("Cereal straw, husks, unprepared, ground, pressed, or in the form of pellets"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01919", "en", DbValue.Nvarchar("Forage products, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01921", "en", DbValue.Nvarchar("Cotton, whether or not ginned"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01922", "en", DbValue.Nvarchar("Jute, kenaf, and other textile bast fibres, raw or retted, except flax, true hemp and ramie"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01929", "en", DbValue.Nvarchar("Other fibre crops, raw, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01930", "en", DbValue.Nvarchar("Plants and parts of plants used primarily in perfumery, in pharmacy, or for insecticidal, fungicidal or similar purposes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01940", "en", DbValue.Nvarchar("Beet seeds (excluding sugar beet seeds) and seeds of forage plants"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01950", "en", DbValue.Nvarchar("Natural rubber in primary forms or in plates, sheets or strip"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01961", "en", DbValue.Nvarchar("Live plants; bulbs, tubers and roots; cuttings and slips; mushroom spawn"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01962", "en", DbValue.Nvarchar("Cut flowers and flower buds including bouquets, wreaths, floral baskets and similar articles"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01963", "en", DbValue.Nvarchar("Flower seeds"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01970", "en", DbValue.Nvarchar("Unmanufactured tobacco"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "01990", "en", DbValue.Nvarchar("Other raw vegetable materials, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02111", "en", DbValue.Nvarchar("Cattle"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02112", "en", DbValue.Nvarchar("Buffalo"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02119", "en", DbValue.Nvarchar("Other bovine animals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02121", "en", DbValue.Nvarchar("Camels and camelids"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02122", "en", DbValue.Nvarchar("Sheep"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02123", "en", DbValue.Nvarchar("Goats"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02129", "en", DbValue.Nvarchar("Other ruminants, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02131", "en", DbValue.Nvarchar("Horses"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02132", "en", DbValue.Nvarchar("Asses"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02133", "en", DbValue.Nvarchar("Mules and hinnies"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02140", "en", DbValue.Nvarchar("Swine / pigs"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02151", "en", DbValue.Nvarchar("Chickens"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02152", "en", DbValue.Nvarchar("Turkeys"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02153", "en", DbValue.Nvarchar("Geese"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02154", "en", DbValue.Nvarchar("Ducks"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02155", "en", DbValue.Nvarchar("Guinea fowls"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02191", "en", DbValue.Nvarchar("Rabbits and hares"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02192", "en", DbValue.Nvarchar("Other mammals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02193", "en", DbValue.Nvarchar("Ostriches and emus"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02194", "en", DbValue.Nvarchar("Other birds"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02195", "en", DbValue.Nvarchar("Reptiles"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02196", "en", DbValue.Nvarchar("Bees"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02199", "en", DbValue.Nvarchar("Other live animals, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02211", "en", DbValue.Nvarchar("Raw milk of cattle"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02212", "en", DbValue.Nvarchar("Raw milk of buffalo"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02291", "en", DbValue.Nvarchar("Raw milk of sheep"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02292", "en", DbValue.Nvarchar("Raw milk of goats"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02293", "en", DbValue.Nvarchar("Raw milk of camel"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02299", "en", DbValue.Nvarchar("Other raw milk n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02311", "en", DbValue.Nvarchar("Hen eggs in shell, fresh, for hatching"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02312", "en", DbValue.Nvarchar("Other hen eggs in shell, fresh"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02321", "en", DbValue.Nvarchar("Eggs from other birds in shell, fresh, for hatching"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02322", "en", DbValue.Nvarchar("Other eggs from other birds in shell, fresh"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02411", "en", DbValue.Nvarchar("Bovine semen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02419", "en", DbValue.Nvarchar("Semen, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02420", "en", DbValue.Nvarchar("Embryos"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02910", "en", DbValue.Nvarchar("Natural honey"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02920", "en", DbValue.Nvarchar("Snails, fresh, chilled, frozen, dried, salted or in brine, except sea snails"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02930", "en", DbValue.Nvarchar("Edible products of animal origin n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02941", "en", DbValue.Nvarchar("Shorn wool, greasy, including fleece-washed shorn wool"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02942", "en", DbValue.Nvarchar("Pulled wool, greasy, including fleece-washed pulled wool; coarse animal hair"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02943", "en", DbValue.Nvarchar("Fine animal hair, not carded or combed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02944", "en", DbValue.Nvarchar("Silk-worm cocoons suitable for reeling"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02951", "en", DbValue.Nvarchar("Raw hides and skins of bovine animals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02952", "en", DbValue.Nvarchar("Raw hides and skins of equine animals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02953", "en", DbValue.Nvarchar("Raw hides and skins of sheep or lambs"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02954", "en", DbValue.Nvarchar("Raw hides and skins of goats or kids"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02955", "en", DbValue.Nvarchar("Raw furskins"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02959", "en", DbValue.Nvarchar("Raw skins of other animals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "02960", "en", DbValue.Nvarchar("Insect waxes and spermaceti, whether or not refined or coloured"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04111", "en", DbValue.Nvarchar("Wild ornamental fish"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04112", "en", DbValue.Nvarchar("Farmed ornamental fish"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04191", "en", DbValue.Nvarchar("Other wild live fish, not for human consumption, including seeds and feeds for aquaculture"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04192", "en", DbValue.Nvarchar("Other farmed live fish, not for human consumption, including seeds and feeds for aquaculture"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04211", "en", DbValue.Nvarchar("Wild freshwater fish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04212", "en", DbValue.Nvarchar("Farmed freshwater fish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04221", "en", DbValue.Nvarchar("Wild salmonidae, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04222", "en", DbValue.Nvarchar("Farmed salmonidae, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04231", "en", DbValue.Nvarchar("Wild flatfish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04232", "en", DbValue.Nvarchar("Farmed flatfish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04241", "en", DbValue.Nvarchar("Wild fish of Gadiformes, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04242", "en", DbValue.Nvarchar("Farmed fish of Gadiformes, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04251", "en", DbValue.Nvarchar("Wild tunas, skipjack or stripe-bellied bonito, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04252", "en", DbValue.Nvarchar("Farmed tunas, skipjack or stripe-bellied bonito, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04261", "en", DbValue.Nvarchar("Other wild pelagic fish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04262", "en", DbValue.Nvarchar("Other farmed pelagic fish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04291", "en", DbValue.Nvarchar("Other wild fish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04292", "en", DbValue.Nvarchar("Other farmed fish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04311", "en", DbValue.Nvarchar("Wild crabs, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04312", "en", DbValue.Nvarchar("Farmed crabs, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04321", "en", DbValue.Nvarchar("Wild rock lobster and other sea crawfish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04322", "en", DbValue.Nvarchar("Farmed rock lobster and other sea crawfish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04331", "en", DbValue.Nvarchar("Wild lobsters (<i>Homarus spp.</i>), live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04332", "en", DbValue.Nvarchar("Farmed lobsters (<i>Homarus spp.</i>), live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04341", "en", DbValue.Nvarchar("Wild Norway lobsters, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04342", "en", DbValue.Nvarchar("Farmed Norway lobsters, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04351", "en", DbValue.Nvarchar("Wild cold-water shrimps and prawns (<i>Pandalus spp.</i>, <i>Crangon crangon</i>), live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04352", "en", DbValue.Nvarchar("Farmed cold-water shrimps and prawns (<i>Pandalus spp.</i>, <i>Crangon crangon</i>), live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04361", "en", DbValue.Nvarchar("Other wild shrimps and prawns, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04362", "en", DbValue.Nvarchar("Other farmed shrimps and prawns, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04391", "en", DbValue.Nvarchar("Other wild crustaceans, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04392", "en", DbValue.Nvarchar("Other farmed crustaceans, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04411", "en", DbValue.Nvarchar("Wild abalone, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04412", "en", DbValue.Nvarchar("Farmed abalone, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04421", "en", DbValue.Nvarchar("Wild oysters, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04422", "en", DbValue.Nvarchar("Farmed oysters, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04431", "en", DbValue.Nvarchar("Wild mussels, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04432", "en", DbValue.Nvarchar("Farmed mussels, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04441", "en", DbValue.Nvarchar("Wild scallops, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04442", "en", DbValue.Nvarchar("Farmed scallops, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04451", "en", DbValue.Nvarchar("Wild clams, cockles and ark shells, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04452", "en", DbValue.Nvarchar("Farmed clams, cockles and ark shells, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04461", "en", DbValue.Nvarchar("Wild cuttle fish and squid, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04462", "en", DbValue.Nvarchar("Farmed cuttle fish and squid, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04471", "en", DbValue.Nvarchar("Wild octopus, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04472", "en", DbValue.Nvarchar("Farmed octopus, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04491", "en", DbValue.Nvarchar("Other wild molluscs, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04492", "en", DbValue.Nvarchar("Other farmed molluscs, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04511", "en", DbValue.Nvarchar("Wild sea cucumbers, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04512", "en", DbValue.Nvarchar("Farmed sea cucumbers, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04521", "en", DbValue.Nvarchar("Wild sea urchins, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04522", "en", DbValue.Nvarchar("Farmed sea urchins, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04530", "en", DbValue.Nvarchar("Jellyfish, live, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04590", "en", DbValue.Nvarchar("Other aquatic invertebrates, live, fresh or chilled, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04911", "en", DbValue.Nvarchar("Coral and similar products, shells of molluscs, crustaceans or echinoderms and cuttle-bone"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04912", "en", DbValue.Nvarchar("Wild live aquatic plants and animals for ornamental purpose"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04913", "en", DbValue.Nvarchar("Farmed live aquatic plants and animals for ornamental purpose"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04920", "en", DbValue.Nvarchar("Natural sponges of aquatic animal origin"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04931", "en", DbValue.Nvarchar("Wild seaweeds and other algae, fresh, frozen or dried, whether or not ground, fit for human consumption"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04932", "en", DbValue.Nvarchar("Farmed seaweeds and other algae, fresh, frozen or dried, whether or not ground, fit for human consumption"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04933", "en", DbValue.Nvarchar("Wild seaweeds and other algae, fresh, frozen or dried, whether or not ground, unfit for human consumption"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "04934", "en", DbValue.Nvarchar("Farmed seaweeds and other algae, fresh, frozen or dried, whether or not ground, unfit for human consumption"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21111", "en", DbValue.Nvarchar("Meat of cattle, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21112", "en", DbValue.Nvarchar("Meat of buffalo, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21113", "en", DbValue.Nvarchar("Meat of pigs, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21114", "en", DbValue.Nvarchar("Meat of rabbits and hares, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21115", "en", DbValue.Nvarchar("Meat of sheep, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21116", "en", DbValue.Nvarchar("Meat of goat, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21117", "en", DbValue.Nvarchar("Meat of camels and camelids, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21118", "en", DbValue.Nvarchar("Meat of horses and other equines, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21119", "en", DbValue.Nvarchar("Other meat of mammals, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21121", "en", DbValue.Nvarchar("Meat of chickens, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21122", "en", DbValue.Nvarchar("Meat of ducks, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21123", "en", DbValue.Nvarchar("Meat of geese, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21124", "en", DbValue.Nvarchar("Meat of turkeys, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21125", "en", DbValue.Nvarchar("Meat of guinea fowl, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21131", "en", DbValue.Nvarchar("Meat of cattle, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21132", "en", DbValue.Nvarchar("Meat of buffalo, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21133", "en", DbValue.Nvarchar("Meat of pigs, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21134", "en", DbValue.Nvarchar("Meat of rabbits and hares, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21135", "en", DbValue.Nvarchar("Meat of sheep, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21136", "en", DbValue.Nvarchar("Meat of goat, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21137", "en", DbValue.Nvarchar("Meat of camels and camelids, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21138", "en", DbValue.Nvarchar("Meat of horses and other equines, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21139", "en", DbValue.Nvarchar("Other meat of mammals, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21141", "en", DbValue.Nvarchar("Meat of chickens, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21142", "en", DbValue.Nvarchar("Meat of ducks, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21143", "en", DbValue.Nvarchar("Meat of geese, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21144", "en", DbValue.Nvarchar("Meat of turkeys, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21145", "en", DbValue.Nvarchar("Meat of guinea fowl, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21151", "en", DbValue.Nvarchar("Edible offal of cattle, fresh, chilled or frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21152", "en", DbValue.Nvarchar("Edible offal of buffalo, fresh, chilled or frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21153", "en", DbValue.Nvarchar("Edible offal of pigs, fresh, chilled or frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21155", "en", DbValue.Nvarchar("Edible offal of sheep, fresh, chilled or frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21156", "en", DbValue.Nvarchar("Edible offal of goat, fresh, chilled or frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21159", "en", DbValue.Nvarchar("Edible offal of mammals, fresh, chilled or frozen, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21160", "en", DbValue.Nvarchar("Edible offal of poultry, fresh, chilled or frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21170", "en", DbValue.Nvarchar("Other meat and edible offal, fresh, chilled or frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21181", "en", DbValue.Nvarchar("Pig meat, cuts, salted, dried or smoked (bacon and ham)"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21182", "en", DbValue.Nvarchar("Bovine meat, salted, dried or smoked"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21183", "en", DbValue.Nvarchar("Other meat and edible meat offal, salted, in brine, dried or smoked; edible flours and meals of meat or meat offal"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21184", "en", DbValue.Nvarchar("Sausages and similar products of meat, offal or blood"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21185", "en", DbValue.Nvarchar("Extracts and juices of meat, fish, crustaceans, molluscs or other aquatic invertebrates"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21186", "en", DbValue.Nvarchar("Prepared dishes and meals based on meat"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21189", "en", DbValue.Nvarchar("Other prepared or preserved meat, meat offal or blood"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21190", "en", DbValue.Nvarchar("Flours, meals and pellets of meat or meat offal, inedible; greaves"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21211", "en", DbValue.Nvarchar("Freshwater fish, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21212", "en", DbValue.Nvarchar("Salmonidae, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21213", "en", DbValue.Nvarchar("Flatfish, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21214", "en", DbValue.Nvarchar("Fish of Gadiformes, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21215", "en", DbValue.Nvarchar("Tunas, skipjack or stripe-bellied bonito, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21216", "en", DbValue.Nvarchar("Other pelagic fish, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21219", "en", DbValue.Nvarchar("Other fish, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21221", "en", DbValue.Nvarchar("Fish fillets and fish meat (whether or not minced), fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21222", "en", DbValue.Nvarchar("Fish fillets, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21223", "en", DbValue.Nvarchar("Fish meat, whether or not minced, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21224", "en", DbValue.Nvarchar("Fish fillets, dried, salted or in brine, but not smoked"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21225", "en", DbValue.Nvarchar("Fish livers and roes, fresh or chilled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21226", "en", DbValue.Nvarchar("Fish livers and roes, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21227", "en", DbValue.Nvarchar("Fish livers and roes dried, smoked, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21231", "en", DbValue.Nvarchar("Fish, dried, but not smoked; salted, but not dried or smoked; or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21232", "en", DbValue.Nvarchar("Fish including fillets, smoked"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21233", "en", DbValue.Nvarchar("Edible fish meal"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21234", "en", DbValue.Nvarchar("Edible fish offal ; fish fins, heads, tails, maws and other edible fish offal"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21241", "en", DbValue.Nvarchar("Prepared dishes and meals based on fish, molluscs and crustaceans"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21242", "en", DbValue.Nvarchar("Fish, otherwise prepared or preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21243", "en", DbValue.Nvarchar("Caviar and caviar substitutes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21251", "en", DbValue.Nvarchar("Crabs, frozen, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21252", "en", DbValue.Nvarchar("Rock lobster and other sea crawfish, frozen, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21253", "en", DbValue.Nvarchar("Lobsters, frozen, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21254", "en", DbValue.Nvarchar("Norway lobsters, frozen, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21255", "en", DbValue.Nvarchar("Cold-water shrimps and prawns, frozen, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21256", "en", DbValue.Nvarchar("Other shrimps and prawns, frozen, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21259", "en", DbValue.Nvarchar("Other crustaceans, frozen, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21261", "en", DbValue.Nvarchar("Abalone, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21262", "en", DbValue.Nvarchar("Oysters, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21263", "en", DbValue.Nvarchar("Mussels, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21264", "en", DbValue.Nvarchar("Scallops, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21265", "en", DbValue.Nvarchar("Clams, cockles and ark shells, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21266", "en", DbValue.Nvarchar("Cuttle fish and squid, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21267", "en", DbValue.Nvarchar("Octopus, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21268", "en", DbValue.Nvarchar("Other molluscs, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21269", "en", DbValue.Nvarchar("Other aquatic invertebrates, frozen, smoked, dried, salted or in brine"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21270", "en", DbValue.Nvarchar("Crustaceans, otherwise prepared or preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21280", "en", DbValue.Nvarchar("Molluscs and other aquatic invertebrates, otherwise prepared or preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21291", "en", DbValue.Nvarchar("Flours, meals and pellets, inedible, of fish, crustaceans, molluscs or other aquatic invertebrates"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21299", "en", DbValue.Nvarchar("Products n.e.c. of fish, crustaceans, molluscs or other aquatic invertebrates; dead fish, crustaceans, molluscs or other aquatic invertebrates unfit for human consumption"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21311", "en", DbValue.Nvarchar("Beans, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21312", "en", DbValue.Nvarchar("Peas, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21313", "en", DbValue.Nvarchar("Potatoes, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21319", "en", DbValue.Nvarchar("Other vegetables and pulses, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21321", "en", DbValue.Nvarchar("Tomato juice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21329", "en", DbValue.Nvarchar("Other vegetable juices"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21330", "en", DbValue.Nvarchar("Vegetables provisionally preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21340", "en", DbValue.Nvarchar("Vegetables, pulses and potatoes, preserved by vinegar or acetic acid"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21391", "en", DbValue.Nvarchar("Prepared dishes and meals based on vegetables, pulses and potatoes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21392", "en", DbValue.Nvarchar("Flour, meal, powder, flakes, granules and pellets of potatoes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21393", "en", DbValue.Nvarchar("Dried potatoes and other dried vegetables"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21394", "en", DbValue.Nvarchar("Potatoes, otherwise prepared or preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21395", "en", DbValue.Nvarchar("Beans, otherwise prepared or preserved, not frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21396", "en", DbValue.Nvarchar("Peas, otherwise prepared or preserved, not frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21397", "en", DbValue.Nvarchar("Mushrooms and truffles, otherwise prepared or preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21399", "en", DbValue.Nvarchar("Other vegetables and pulses, preserved other than by vinegar, acetic acid or sugar, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21411", "en", DbValue.Nvarchar("Raisins"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21412", "en", DbValue.Nvarchar("Plums, dried"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21419", "en", DbValue.Nvarchar("Other dried fruit, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21421", "en", DbValue.Nvarchar("Groundnuts, shelled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21422", "en", DbValue.Nvarchar("Almonds, shelled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21423", "en", DbValue.Nvarchar("Hazelnuts, shelled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21424", "en", DbValue.Nvarchar("Cashew nuts, shelled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21429", "en", DbValue.Nvarchar("Other shelled nuts"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21431", "en", DbValue.Nvarchar("Orange juice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21432", "en", DbValue.Nvarchar("Grapefruit juice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21433", "en", DbValue.Nvarchar("Pineapple juice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21434", "en", DbValue.Nvarchar("Grape juice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21435", "en", DbValue.Nvarchar("Apple juice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21439", "en", DbValue.Nvarchar("Other fruit juices, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21491", "en", DbValue.Nvarchar("Pineapples, otherwise prepared or preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21492", "en", DbValue.Nvarchar("Peaches, otherwise prepared or preserved"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21493", "en", DbValue.Nvarchar("Fruits and nuts, uncooked or cooked, frozen"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21494", "en", DbValue.Nvarchar("Jams, fruit jellies, marmalades, fruit or nut purree and fruit or nut pastes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21495", "en", DbValue.Nvarchar("Nuts, groundnuts and other seeds, roasted, salted or otherwise prepared n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21496", "en", DbValue.Nvarchar("Fruits and nuts, provisionally preserved, not for immediate consumption"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21499", "en", DbValue.Nvarchar("Other prepared and preserved fruits and nuts, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21511", "en", DbValue.Nvarchar("Fats, of pig and poultry, unrendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21512", "en", DbValue.Nvarchar("Cattle fat, unrendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21513", "en", DbValue.Nvarchar("Buffalo fat, unrendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21514", "en", DbValue.Nvarchar("Sheep fat, unrendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21515", "en", DbValue.Nvarchar("Goat fat, unrendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21519", "en", DbValue.Nvarchar("Other animal fats, unrendered, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21521", "en", DbValue.Nvarchar("Pig fat, rendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21522", "en", DbValue.Nvarchar("Poultry fat, rendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21523", "en", DbValue.Nvarchar("Tallow"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21524", "en", DbValue.Nvarchar("Fish-liver oils and their fractions"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21525", "en", DbValue.Nvarchar("Fats and oils and their fractions, of fish, other than liver oils"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21526", "en", DbValue.Nvarchar("Fats and oils and their fractions, of marine mammals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21529", "en", DbValue.Nvarchar("Other animal fats, rendered"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21590", "en", DbValue.Nvarchar("Animal fats and their fractions, partly or wholly hydrogenated, inter-esterified, re-esterified or elaidinised, whether or not refined, but not further prepared"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21611", "en", DbValue.Nvarchar("Soya bean oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21612", "en", DbValue.Nvarchar("Soya bean oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21621", "en", DbValue.Nvarchar("Groundnut oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21622", "en", DbValue.Nvarchar("Groundnut oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21631", "en", DbValue.Nvarchar("Sunflower-seed and safflower-seed oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21632", "en", DbValue.Nvarchar("Sunflower-seed and safflower-seed oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21641", "en", DbValue.Nvarchar("Rape, colza and mustard oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21642", "en", DbValue.Nvarchar("Rape, colza and mustard oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21651", "en", DbValue.Nvarchar("Palm oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21652", "en", DbValue.Nvarchar("Palm oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21661", "en", DbValue.Nvarchar("Coconut oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21662", "en", DbValue.Nvarchar("Coconut oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21671", "en", DbValue.Nvarchar("Olive oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21672", "en", DbValue.Nvarchar("Olive oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21673", "en", DbValue.Nvarchar("Oil of olive residues"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21681", "en", DbValue.Nvarchar("Cottonseed oil, crude"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21682", "en", DbValue.Nvarchar("Cottonseed oil, refined"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21691", "en", DbValue.Nvarchar("Other vegetable oils, crude or refined, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21693", "en", DbValue.Nvarchar("Vegetable oils and their fractions, partly or wholly hydrogenated, inter-esterified, re-esterified or elaidinised, whether or not refined, but not further prepared"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21700", "en", DbValue.Nvarchar("Margarine and similar preparations"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21800", "en", DbValue.Nvarchar("Cotton linters"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21910", "en", DbValue.Nvarchar("Oil-cake and other solid residues, of vegetable fats or oils"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21920", "en", DbValue.Nvarchar("Flours and meals of oil seeds or oleaginous fruits, except those of mustard"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21931", "en", DbValue.Nvarchar("Vegetable waxes (other than triglycerides), whether or not refined or coloured"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "21932", "en", DbValue.Nvarchar("Degras; residues resulting from the treatment of fatty substances or animal or vegetable waxes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22110", "en", DbValue.Nvarchar("Processed liquid milk"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22120", "en", DbValue.Nvarchar("Cream, fresh"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22130", "en", DbValue.Nvarchar("Whey"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22211", "en", DbValue.Nvarchar("Whole milk powder"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22212", "en", DbValue.Nvarchar("Skim milk and whey powder"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22219", "en", DbValue.Nvarchar("Other milk and cream in solid forms, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22221", "en", DbValue.Nvarchar("Evaporated milk"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22222", "en", DbValue.Nvarchar("Condensed milk"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22229", "en", DbValue.Nvarchar("Milk and cream, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22230", "en", DbValue.Nvarchar("Yoghurt and other fermented or acidified milk and cream"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22241", "en", DbValue.Nvarchar("Butter and other fats and oils derived from milk of cattle"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22242", "en", DbValue.Nvarchar("Butter and other fats and oils derived from milk of buffalo"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22249", "en", DbValue.Nvarchar("Butter and other fats and oils derived from milk of other animals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22251", "en", DbValue.Nvarchar("Cheese from milk of cattle, fresh or processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22252", "en", DbValue.Nvarchar("Cheese from milk of buffalo, fresh or processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22253", "en", DbValue.Nvarchar("Cheese from milk of sheep, fresh or processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22254", "en", DbValue.Nvarchar("Cheese from milk of goats, fresh or processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22259", "en", DbValue.Nvarchar("Cheese, fresh or processed, n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22260", "en", DbValue.Nvarchar("Casein"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22270", "en", DbValue.Nvarchar("Ice cream and other edible ice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22290", "en", DbValue.Nvarchar("Dairy products n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "22300", "en", DbValue.Nvarchar("Eggs, in shell, preserved or cooked"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23110", "en", DbValue.Nvarchar("Wheat and meslin flour"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23120", "en", DbValue.Nvarchar("Other cereal flours"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23130", "en", DbValue.Nvarchar("Groats, meal and pellets of wheat and other cereals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23140", "en", DbValue.Nvarchar("Other cereal grain products (including corn flakes)"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23161", "en", DbValue.Nvarchar("Rice, semi- or wholly milled"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23162", "en", DbValue.Nvarchar("Husked rice"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23170", "en", DbValue.Nvarchar("Other vegetable flours and meals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23180", "en", DbValue.Nvarchar("Mixes and doughs for the preparation of bakers' wares"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23210", "en", DbValue.Nvarchar("Glucose and glucose syrup; fructose and fructose syrup; lactose and lactose syrup; invert sugar; sugars and sugar syrups n.e.c.; artificial honey; caramel"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23220", "en", DbValue.Nvarchar("Starches; inulin; wheat gluten; dextrins and other modified starches"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23230", "en", DbValue.Nvarchar("Tapioca and substitutes therefor prepared from starch, in the form of flakes, grains, siftings or similar forms"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23311", "en", DbValue.Nvarchar("Dog or cat food, put up for retail sale"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23319", "en", DbValue.Nvarchar("Preparations used in animal feeding n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23320", "en", DbValue.Nvarchar("Lucerne (alfalfa) meal and pellets"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23410", "en", DbValue.Nvarchar("Crispbread; rusks, toasted bread and similar toasted products"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23420", "en", DbValue.Nvarchar("Gingerbread and the like; sweet biscuits; waffles and wafers"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23430", "en", DbValue.Nvarchar("Pastry goods and cakes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23490", "en", DbValue.Nvarchar("Bread and other bakers' wares"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23511", "en", DbValue.Nvarchar("Cane sugar"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23512", "en", DbValue.Nvarchar("Beet sugar"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23520", "en", DbValue.Nvarchar("Refined sugar"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23530", "en", DbValue.Nvarchar("Refined cane or beet sugar, in solid form, containing added flavouring or colouring matter; maple sugar and maple syrup"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23540", "en", DbValue.Nvarchar("Molasses"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23610", "en", DbValue.Nvarchar("Cocoa paste, whether or not defatted"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23620", "en", DbValue.Nvarchar("Cocoa butter, fat and oil"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23630", "en", DbValue.Nvarchar("Cocoa powder, not sweetened"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23640", "en", DbValue.Nvarchar("Cocoa powder, sweetened"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23650", "en", DbValue.Nvarchar("Chocolate and other food preparations containing cocoa (except sweetened cocoa powder), in bulk forms"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23660", "en", DbValue.Nvarchar("Chocolate and other food preparations containing cocoa (except sweetened cocoa powder), other than in bulk forms"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23670", "en", DbValue.Nvarchar("Sugar confectionery (including white chocolate), not containing cocoa; vegetables, fruits, nuts, fruit-peel and other parts of plants, preserved by sugar"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23710", "en", DbValue.Nvarchar("Uncooked pasta, not stuffed or otherwise prepared"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23721", "en", DbValue.Nvarchar("Pasta, cooked, stuffed or otherwise prepared (but not as a complete dish); couscous, except as a complete dish"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23722", "en", DbValue.Nvarchar("Prepared dishes containing stuffed pasta; prepared couscous dishes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23911", "en", DbValue.Nvarchar("Coffee, decaffeinated or roasted"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23912", "en", DbValue.Nvarchar("Coffee substitutes containing coffee; extracts, essences and concentrates of coffee, and preparations with a basis thereof or with a basis of coffee; roasted chicory and other roasted coffee substitutes, and extracts, essences and concentrates thereof"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23913", "en", DbValue.Nvarchar("Green tea (not fermented), black tea (fermented) and partly fermented tea, in immediate packings of a content not exceeding 3 kg"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23914", "en", DbValue.Nvarchar("Extracts, essences and concentrates of tea or matÃ©, and preparations with a basis thereof or with a basis of tea or matÃ©"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23921", "en", DbValue.Nvarchar("Pepper (piper spp.), processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23922", "en", DbValue.Nvarchar("Chillies and peppers, dry (capsicum spp., pimenta), processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23923", "en", DbValue.Nvarchar("Nutmeg, mace, cardamoms, processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23924", "en", DbValue.Nvarchar("Anise, badian, coriander, cumin, caraway, fennel and juniper berries, processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23925", "en", DbValue.Nvarchar("Cinnamon (canella), processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23926", "en", DbValue.Nvarchar("Cloves (whole stems), processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23927", "en", DbValue.Nvarchar("Ginger, processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23928", "en", DbValue.Nvarchar("Vanilla, processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23929", "en", DbValue.Nvarchar("Other spices and aromatics, processed"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23991", "en", DbValue.Nvarchar("Homogenized preparations of meat, vegetables, fruits or nuts; preparations of milk, flour, meal, starch or malt extract, for infant use n.e.c.; homogenized composite food preparations"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23992", "en", DbValue.Nvarchar("Soups and broths and preparations thereof"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23993", "en", DbValue.Nvarchar("Eggs, not in shell, and egg yolks, fresh or preserved; egg albumin"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23994", "en", DbValue.Nvarchar("Vinegar and substitutes therefor obtained from acetic acid"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23995", "en", DbValue.Nvarchar("Sauces; mixed condiments; mustard flour and meal; prepared mustard"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23996", "en", DbValue.Nvarchar("Yeasts (active or inactive); other single-cell micro-organisms, dead; prepared baking powders"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23997", "en", DbValue.Nvarchar("Other prepared dishes and meals"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "23999", "en", DbValue.Nvarchar("Other food products n.e.c."), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24110", "en", DbValue.Nvarchar("Undenatured ethyl alcohol of an alcoholic strength by volume of 80% vol or higher"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24131", "en", DbValue.Nvarchar("Spirits, liqueurs and other spirituous beverages of an alcoholic strength by volume of about 40% vol"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24139", "en", DbValue.Nvarchar("Other spirituous beverages and undenatured ethyl alcohol of an alcoholic strength by volume of less than 80% vol"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24211", "en", DbValue.Nvarchar("Sparkling wine of fresh grapes"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24212", "en", DbValue.Nvarchar("Wine of fresh grapes, except sparkling wine; grape must"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24220", "en", DbValue.Nvarchar("Vermouth and other wine of fresh grapes flavoured with plats or aromatic substances"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24230", "en", DbValue.Nvarchar("Cider, perry, mead and other fermented beverages, except wine of fresh grapes and beer made from malt"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24310", "en", DbValue.Nvarchar("Beer made from malt"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24320", "en", DbValue.Nvarchar("Malt, whether or not roasted"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24410", "en", DbValue.Nvarchar("Bottled waters, not sweetened or flavoured"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", "24490", "en", DbValue.Nvarchar("Other non-alcoholic caloric beverages"), 0);

            #endregion

            #region RESOURCE TYPE

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                .Values(10, "0", "99510", "en", DbValue.Nvarchar("Resource Type"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(4, "99510", "FOOD", "en", DbValue.Nvarchar("Food"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(4, "99510", "NONFOOD", "en", DbValue.Nvarchar("Non food"), 0);

            #endregion

            #region DXDIR_BUSINESS_COST.TITLE

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "ELECTRICITY", "en", DbValue.Nvarchar("Electricity"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "GAS", "en", DbValue.Nvarchar("Gas"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "RENT", "en", DbValue.Nvarchar("Rent"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "WAGES", "en", DbValue.Nvarchar("Wages"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "MANAGEMENT", "en", DbValue.Nvarchar("Management"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "FACILITIES", "en", DbValue.Nvarchar("Facilities"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "STORAGE", "en", DbValue.Nvarchar("Storage"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "FREIGHT", "en", DbValue.Nvarchar("Freight"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                    .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                    .Values(11, "DXDIR_BUSINESS_COST.TITLE", "WASTE", "en", DbValue.Nvarchar("Waste & loss collection and treatment"), 0);

            Database.Data.InsertInto("DX_PHRASE_TEXT")
                   .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
                   .Values(11, "DXDIR_BUSINESS_COST.TITLE", "MATLOSS", "en", DbValue.Nvarchar("Material losses (incl those that make revenue)"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				   .Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				   .Values(11, "DXDIR_BUSINESS_COST.TITLE", "OTHER", "en", DbValue.Nvarchar("Other"), 0);

			#endregion

			#region DXDIR_RESULTS

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "UTILISED_INPUT", "en", DbValue.Nvarchar("Material inputs to gross product ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "STOCK_HOLDING", "en", DbValue.Nvarchar("Non product material to gross product ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "FOOD_LOSS", "en", DbValue.Nvarchar("Food material loss (with inedible parts) to total material loss ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "FOOD_EDIBLE_LOSS_INPUTS", "en", DbValue.Nvarchar("Food material loss (without inedible parts) to inputs ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "FOOD_EDIBLE_LOSS", "en", DbValue.Nvarchar("Food material loss (without inedible parts) to total material loss ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "LOSS_EX_WATER", "en", DbValue.Nvarchar("Total material loss to material input ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "PACK_LOSS", "en", DbValue.Nvarchar("Packaging material loss to food input ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "OUTPUT_PACK", "en", DbValue.Nvarchar("Output product packaging to gross product ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "ESTIM_COST", "en", DbValue.Nvarchar("Estimated true cost of waste"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "WASTE_REMOVAL", "en", DbValue.Nvarchar("Waste removal cost"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "ENERGY_SPEND", "en", DbValue.Nvarchar("Energy spend to operating cost ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "MAT_SPEND", "en", DbValue.Nvarchar("Material spend to operating cost ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "DISPOSAL", "en", DbValue.Nvarchar("Disposal to purchase ratio"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", "ESTIM_COST_WASTE", "en", DbValue.Nvarchar("Estimated true cost of waste ratio to purchase ratio"), 0);
			#endregion DXDIR_RESULTS

			#region DXDIR_DATA_QUALITY

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(10, "0", "99511", "en", DbValue.Nvarchar("Data Quality"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(4, "99511", "1", "en", DbValue.Nvarchar("Measured"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(4, "99511", "2", "en", DbValue.Nvarchar("Projected"), 0);

			Database.Data.InsertInto("DX_PHRASE_TEXT")
				.Columns("TYPE", "SUBTYPE", "CODE", "LANGUAGE", "TEXT", "FLAGS")
				.Values(4, "99511", "3", "en", DbValue.Nvarchar("Estimated"), 0);

			#endregion
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
