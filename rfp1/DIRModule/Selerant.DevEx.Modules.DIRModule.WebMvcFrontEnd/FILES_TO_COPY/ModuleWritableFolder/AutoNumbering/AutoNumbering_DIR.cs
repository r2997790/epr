// AutoNumbering_DIR.cs
// This script allows the customization of the auto-generation of codes for DIR module specific entities.
// This script is part of the Selerant.DevEx.UserScripting namespace
// You can access any namespace or class in DevEX, provided it is imported or the fully qualified name is specified.

using System.Reflection;
using System;
using System.Text;
using Selerant.DevEx.Dal.BizObj;
using Selerant.DevEx.Dal.TypedDataSets;
using Selerant.DevEx.Scripting;
using Selerant.DevEx.BusinessLayer;

public class AutoNumbering_DIR
{
	public string CurrentUserName { get; set; }

	/// <summary>
	/// Reserve key for the Direct Assessment being created. This function can be changed by the end-user
	/// to customized the standard numbering creation.
	/// <summary>
	/// <param name="assessmentTypeCode">Can be used to implement adding prefix or suffix to reserved key</param>
	/// <returns>Returns a concatenated string "KEYLIST<tab>NEWKEY"</returns>
	public string AssessmentNewKey(string assessmentTypeCode)
	{
		string keyList = "STANDARD";
		string keyBase = "00000000"; // This is significant only when no sequence is stored in the db yet.

		// Call the standard DAL method to reserve a key in a sequence of keys
		string newKey = Selerant.DevEx.Dal.BizObj.BizDsAutoNumbering.ObtainAndReserveNewKey("ASSESSMENT", keyList, keyBase);

		// Must return KEYLIST<tab>NEWKEY
		return (keyList + "\t" + newKey);
	}

    /// <summary>
	/// Reserve key for the Direct Assessment Type being created. This function can be changed by the end-user
	/// to customized the standard numbering creation.
	/// <summary>
	/// <param name="assessmentTypeCode">Can be used to implement adding prefix or suffix to reserved key</param>
	/// <returns>Returns a concatenated string "KEYLIST<tab>NEWKEY"</returns>
    public string AssessmentTypeNewKey(string assessmentTypeCode)
    {
        string keyList = "STANDARD";
        string keyBase = "00000000"; // This is significant only when no sequence is stored in the db yet.

        // Call the standard DAL method to reserve a key in a sequence of keys
        string newKey = Selerant.DevEx.Dal.BizObj.BizDsAutoNumbering.ObtainAndReserveNewKey("ASSESSMENT_TYPE", keyList, keyBase);

        // Must return KEYLIST<tab>NEWKEY
        return (keyList + "\t" + newKey);
    }
}