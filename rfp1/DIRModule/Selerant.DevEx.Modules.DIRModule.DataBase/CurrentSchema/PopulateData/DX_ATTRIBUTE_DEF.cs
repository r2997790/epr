using Selerant.ApplicationBlocks.Data.Migrations;
using Selerant.ApplicationBlocks.Data.Migrations.Model.Data;
using System;

namespace Selerant.DevEx.Modules.DIRModule.DataBase.CurrentSchema.PopulateData
{
    public  class DX_ATTRIBUTE_DEF : MigrationBase
    {
        public override void Up()
        {
            Database.Data.InsertInto("DX_ATTRIBUTE_DEF")
                .Columns("ID", "NAME", "SCOPE", "TYPE", "LENGTH", "CATEGORY", "LIST_ID", "ARRAY_SIZE", "MANDATORY", "DEF_VALUE", "STORAGE", "SYSTEM", "READONLY", "FLAGS")
                .Values(99500, "DXDIR_ASSESSMENT_COMMENT", "ASSESSMENT", "STRING", "2000", DbValue.Null, DbValue.Null, 1, DbValue.Null, DbValue.Null, "DXDIR_ATTVALUE_ASSESSMENT", 1, 0, 0);

            Database.Data.InsertInto("DX_ATTRIBUTE_DEF")
                .Columns("ID", "NAME", "SCOPE", "TYPE", "LENGTH", "CATEGORY", "LIST_ID", "ARRAY_SIZE", "MANDATORY", "DEF_VALUE", "STORAGE", "SYSTEM", "READONLY", "FLAGS")
                .Values(99510, "DXDIR_RESOURCE_TYPE", "MATERIAL", "LISTOFVAL", "32", DbValue.Null, DbValue.Null, 1, DbValue.Null, DbValue.Null, "DX_ATTVALUE_MATERIAL", 0, 0, 0);

			Database.Data.InsertInto("DX_ATTRIBUTE_DEF")
			    .Columns("ID", "NAME", "SCOPE", "TYPE", "LENGTH", "CATEGORY", "LIST_ID", "ARRAY_SIZE", "MANDATORY", "DEF_VALUE", "STORAGE", "SYSTEM", "READONLY", "FLAGS")
			    .Values(99511, "DXDIR_DATA_QUALITY", "ASSESSMENT", "LISTOFVAL", "2000", DbValue.Null, DbValue.Null, 1, DbValue.Null, DbValue.Null, "DXDIR_ATTVALUE_ASSESSMENT", 1, 0, 0);
		}

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
