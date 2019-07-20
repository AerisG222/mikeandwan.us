using System;
using System.Collections.Generic;
using System.Globalization;


namespace MawMvcApp.ViewModels.Tools.Dotnet
{
	public class FormatExampleManager
	{
		public IEnumerable<FormatExampleGroup> GetDateFormatExamples(DateTime date)
		{
			var groupList = new List<FormatExampleGroup>();

			var group = new FormatExampleGroup("Standard Formatting Methods");

			group.FormatExampleList.Add(new FormatExample("date.ToLongDateString()", date.ToLongDateString()));
			group.FormatExampleList.Add(new FormatExample("date.ToLongTimeString()", date.ToLongTimeString()));
			group.FormatExampleList.Add(new FormatExample("date.ToShortDateString()", date.ToShortDateString()));
			group.FormatExampleList.Add(new FormatExample("date.ToShortTimeString()", date.ToShortTimeString()));
			group.FormatExampleList.Add(new FormatExample("date.ToString()", date.ToString(CultureInfo.InvariantCulture)));
			groupList.Add(group);

			group = new FormatExampleGroup("Predefined Formats");
			group.FormatExampleList.Add(new FormatExample("d", date.ToString("d", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("D", date.ToString("D", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("f", date.ToString("f", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("F", date.ToString("F", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("g", date.ToString("g", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("G", date.ToString("G", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("m -or- M", date.ToString("M", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("o -or- O", date.ToString("O", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("r -or- R", date.ToString("r", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("s", date.ToString("s", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("t", date.ToString("t", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("T", date.ToString("T", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("u", date.ToString("u", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("U", date.ToString("U", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("y -or- Y", date.ToString("y", CultureInfo.InvariantCulture)));
			groupList.Add(group);

			group = new FormatExampleGroup("Custom Format Strings");
			group.FormatExampleList.Add(new FormatExample("d -or- %d", date.ToString("%d", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("dd", date.ToString("dd", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("ddd", date.ToString("ddd", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("dddd", date.ToString("dddd", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("M -or- %M", date.ToString("%M", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("MM", date.ToString("MM", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("MMM", date.ToString("MMM", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("MMMM", date.ToString("MMMM", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("y -or- %y", date.ToString("%y", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("yy", date.ToString("yy", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("yyy", date.ToString("yyy", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("yyyy", date.ToString("yyyy", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("yyyyy", date.ToString("yyyyy", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("gg", date.ToString("gg", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("h -or- %h", date.ToString("%h", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("hh", date.ToString("hh", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("H -or- %H", date.ToString("%H", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("HH", date.ToString("HH", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("m -or- %m", date.ToString("%m", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("mm", date.ToString("mm", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("s -or- %s", date.ToString("%s", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("ss", date.ToString("ss", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("f -or- %f", date.ToString("%f", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("ff", date.ToString("ff", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("fff", date.ToString("fff", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("ffff", date.ToString("ffff", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("fffff", date.ToString("fffff", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("ffffff", date.ToString("ffffff", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("fffffff", date.ToString("fffffff", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("F -or- %F", date.ToString("%F", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("FF", date.ToString("FF", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("FFF", date.ToString("FFF", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("FFFF", date.ToString("FFFF", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("FFFFF", date.ToString("FFFFF", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("FFFFFF", date.ToString("FFFFFF", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("FFFFFFF", date.ToString("FFFFFFF", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("t -or- %t", date.ToString("%t", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("tt", date.ToString("tt", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("z -or- %z", date.ToString("%z", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("zz", date.ToString("zz", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("zzz", date.ToString("zzz", CultureInfo.InvariantCulture)));
			groupList.Add(group);

			return groupList;
		}


		public IEnumerable<FormatExampleGroup> GetNumberFormatExamples(double value)
		{
			var groupList = new List<FormatExampleGroup>();

			var group = new FormatExampleGroup("Standard Formatting Methods");
			group.FormatExampleList.Add(new FormatExample("value.ToString()", value.ToString(CultureInfo.InvariantCulture)));
			groupList.Add(group);

			group = new FormatExampleGroup("Predefined Formats");
			group.FormatExampleList.Add(new FormatExample("c -or- C", value.ToString("c", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("d -or- D (int only)", ((int)value).ToString("d", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("d2 -or- D2 (int only)", ((int)value).ToString("d2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("d6 -or- D6 (int only)", ((int)value).ToString("d6", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("e", value.ToString("e", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("e2", value.ToString("e2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("e6", value.ToString("e6", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("E", value.ToString("E", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("E2", value.ToString("E2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("E6", value.ToString("E6", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("f -or- F", value.ToString("f", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("f2 -or- F2", value.ToString("f2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("f6 -or- F6", value.ToString("f6", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("g -or- G", value.ToString("g", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("n -or- N", value.ToString("n", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("n2 -or- N2", value.ToString("n2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("n6 -or- N6", value.ToString("n6", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("p -or- P", value.ToString("p", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("p2 -or- P2", value.ToString("p2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("p6 -or- P6", value.ToString("p6", CultureInfo.InvariantCulture)));
			group.FormatExampleList.Add(new FormatExample("r -or- R", value.ToString("R", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("x (int only)", ((int)value).ToString("x", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("x2 (int only)", ((int)value).ToString("x2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("x6 (int only)", ((int)value).ToString("x6", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("X (int only)", ((int)value).ToString("X", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("X2 (int only)", ((int)value).ToString("X2", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("X6 (int only)", ((int)value).ToString("X6", CultureInfo.InvariantCulture)));
			groupList.Add(group);

			group = new FormatExampleGroup("Custom Format Strings");
			group.FormatExampleList.Add(new FormatExample("0.00", value.ToString("0.00", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("#.00", value.ToString("#.00", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("#,###.##", value.ToString("#,###.##", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("0,000,000.00", value.ToString("0,000,000.00", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("#,###,###.00", value.ToString("#,###,###.00", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("(#,###.##)", value.ToString("(#,###.##)", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("0.00%", value.ToString("0.00%", CultureInfo.InvariantCulture)));
	        group.FormatExampleList.Add(new FormatExample("#.##;(#.##);--", value.ToString("#.##;(#.##);--", CultureInfo.InvariantCulture)));
			groupList.Add(group);

			return groupList;
		}
	}
}
