using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
    public class DX_PHRASE_TYPE : MigrationBase
    {
        public override void Up()
        {
            Database.Data.InsertInto("DX_PHRASE_TYPE")
                .Columns("TYPE", "SUBTYPE", "DES", "CODE_LEN", "TEXT_LEN", "DEFAULT_LANG", "SUPPORT_LANG", "FLAGS")
                .Values(11, "DXDIR_ASSESSMENT.STATUS", DbValue.Nvarchar("DXDIR_ASSESSMENT.STATUS"), 18, 50, "en", null, 0);

			Database.Data.InsertInto("DX_PHRASE_TYPE")
					.Columns("TYPE", "SUBTYPE", "DES", "CODE_LEN", "TEXT_LEN", "DEFAULT_LANG", "SUPPORT_LANG", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.ORG_STRUCTURE", DbValue.Nvarchar("DXDIR_ASSESSMENT.ORG_STRUCTURE"), 32, 50, "en", null, 0);

			Database.Data.InsertInto("DX_PHRASE_TYPE")
					.Columns("TYPE", "SUBTYPE", "DES", "CODE_LEN", "TEXT_LEN", "DEFAULT_LANG", "SUPPORT_LANG", "FLAGS")
					.Values(11, "DXDIR_ASSESSMENT.PROD_CLASSIF", DbValue.Nvarchar("DXDIR_ASSESSMENT.PROD_CLASSIF"), 32, 50, "en", null, 0);

            Database.Data.InsertInto("DX_PHRASE_TYPE")
                    .Columns("TYPE", "SUBTYPE", "DES", "CODE_LEN", "TEXT_LEN", "DEFAULT_LANG", "SUPPORT_LANG", "FLAGS")
                    .Values(4, "99510", DbValue.Nvarchar("DIRECT Resource Type"), 32, 50, "en", null, 0);

            Database.Data.InsertInto("DX_PHRASE_TYPE")
                .Columns("TYPE", "SUBTYPE", "DES", "CODE_LEN", "TEXT_LEN", "DEFAULT_LANG", "SUPPORT_LANG", "FLAGS")
                .Values(11, "DXDIR_BUSINESS_COST.TITLE", DbValue.Nvarchar("DXDIR_BUSINESS_COST.TITLE"), 32, 50, "en", null, 0);

			Database.Data.InsertInto("DX_PHRASE_TYPE")
				.Columns("TYPE", "SUBTYPE", "DES", "CODE_LEN", "TEXT_LEN", "DEFAULT_LANG", "SUPPORT_LANG", "FLAGS")
				.Values(11, "DXDIR_RESULT.TITLE", DbValue.Nvarchar("DXDIR_RESULT.TITLE"), 32, 50, "en", null, 0);

			Database.Data.InsertInto("DX_PHRASE_TYPE")
				.Columns("TYPE", "SUBTYPE", "DES", "CODE_LEN", "TEXT_LEN", "DEFAULT_LANG", "SUPPORT_LANG", "FLAGS")
				.Values(4, "99511", DbValue.Nvarchar("DIRECT Data Quality"), 32, 50, "en", null, 0);
		}

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
