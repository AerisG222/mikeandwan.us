using System;
using System.Collections.Generic;


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
			group.FormatExampleList.Add(new FormatExample("date.ToString()", date.ToString()));
			groupList.Add(group);
			
			group = new FormatExampleGroup("Predefined Formats");
			group.FormatExampleList.Add(new FormatExample("d", date.ToString("d")));
			group.FormatExampleList.Add(new FormatExample("D", date.ToString("D")));
			group.FormatExampleList.Add(new FormatExample("f", date.ToString("f")));
			group.FormatExampleList.Add(new FormatExample("F", date.ToString("F")));
			group.FormatExampleList.Add(new FormatExample("g", date.ToString("g")));
			group.FormatExampleList.Add(new FormatExample("G", date.ToString("G")));
			group.FormatExampleList.Add(new FormatExample("m -or- M", date.ToString("M")));
			group.FormatExampleList.Add(new FormatExample("o -or- O", date.ToString("O")));
			group.FormatExampleList.Add(new FormatExample("r -or- R", date.ToString("r")));
			group.FormatExampleList.Add(new FormatExample("s", date.ToString("s")));
			group.FormatExampleList.Add(new FormatExample("t", date.ToString("t")));
			group.FormatExampleList.Add(new FormatExample("T", date.ToString("T")));
			group.FormatExampleList.Add(new FormatExample("u", date.ToString("u")));
			group.FormatExampleList.Add(new FormatExample("U", date.ToString("U")));
			group.FormatExampleList.Add(new FormatExample("y -or- Y", date.ToString("y")));
			groupList.Add(group);
			
			group = new FormatExampleGroup("Custom Format Strings");
			group.FormatExampleList.Add(new FormatExample("d -or- %d", date.ToString("%d")));
			group.FormatExampleList.Add(new FormatExample("dd", date.ToString("dd")));
			group.FormatExampleList.Add(new FormatExample("ddd", date.ToString("ddd")));
			group.FormatExampleList.Add(new FormatExample("dddd", date.ToString("dddd")));
			group.FormatExampleList.Add(new FormatExample("M -or- %M", date.ToString("%M")));
			group.FormatExampleList.Add(new FormatExample("MM", date.ToString("MM")));
			group.FormatExampleList.Add(new FormatExample("MMM", date.ToString("MMM")));
			group.FormatExampleList.Add(new FormatExample("MMMM", date.ToString("MMMM")));
			group.FormatExampleList.Add(new FormatExample("y -or- %y", date.ToString("%y")));
			group.FormatExampleList.Add(new FormatExample("yy", date.ToString("yy")));
			group.FormatExampleList.Add(new FormatExample("yyy", date.ToString("yyy")));
			group.FormatExampleList.Add(new FormatExample("yyyy", date.ToString("yyyy")));
			group.FormatExampleList.Add(new FormatExample("yyyyy", date.ToString("yyyyy")));
			group.FormatExampleList.Add(new FormatExample("gg", date.ToString("gg")));
			group.FormatExampleList.Add(new FormatExample("h -or- %h", date.ToString("%h")));
			group.FormatExampleList.Add(new FormatExample("hh", date.ToString("hh")));
			group.FormatExampleList.Add(new FormatExample("H -or- %H", date.ToString("%H")));
			group.FormatExampleList.Add(new FormatExample("HH", date.ToString("HH")));
			group.FormatExampleList.Add(new FormatExample("m -or- %m", date.ToString("%m")));
			group.FormatExampleList.Add(new FormatExample("mm", date.ToString("mm")));
			group.FormatExampleList.Add(new FormatExample("s -or- %s", date.ToString("%s")));
			group.FormatExampleList.Add(new FormatExample("ss", date.ToString("ss")));
			group.FormatExampleList.Add(new FormatExample("f -or- %f", date.ToString("%f")));
			group.FormatExampleList.Add(new FormatExample("ff", date.ToString("ff")));
			group.FormatExampleList.Add(new FormatExample("fff", date.ToString("fff")));
			group.FormatExampleList.Add(new FormatExample("ffff", date.ToString("ffff")));
			group.FormatExampleList.Add(new FormatExample("fffff", date.ToString("fffff")));
			group.FormatExampleList.Add(new FormatExample("ffffff", date.ToString("ffffff")));
			group.FormatExampleList.Add(new FormatExample("fffffff", date.ToString("fffffff")));
			group.FormatExampleList.Add(new FormatExample("F -or- %F", date.ToString("%F")));
			group.FormatExampleList.Add(new FormatExample("FF", date.ToString("FF")));
			group.FormatExampleList.Add(new FormatExample("FFF", date.ToString("FFF")));
			group.FormatExampleList.Add(new FormatExample("FFFF", date.ToString("FFFF")));
			group.FormatExampleList.Add(new FormatExample("FFFFF", date.ToString("FFFFF")));
			group.FormatExampleList.Add(new FormatExample("FFFFFF", date.ToString("FFFFFF")));
			group.FormatExampleList.Add(new FormatExample("FFFFFFF", date.ToString("FFFFFFF")));
			group.FormatExampleList.Add(new FormatExample("t -or- %t", date.ToString("%t")));
			group.FormatExampleList.Add(new FormatExample("tt", date.ToString("tt")));
			group.FormatExampleList.Add(new FormatExample("z -or- %z", date.ToString("%z")));
			group.FormatExampleList.Add(new FormatExample("zz", date.ToString("zz")));
			group.FormatExampleList.Add(new FormatExample("zzz", date.ToString("zzz")));
			groupList.Add(group);
			
			return groupList;
		}
		
		
		public IEnumerable<FormatExampleGroup> GetNumberFormatExamples(double value)
		{
			var groupList = new List<FormatExampleGroup>();
			
			var group = new FormatExampleGroup("Standard Formatting Methods");
			group.FormatExampleList.Add(new FormatExample("value.ToString()", value.ToString()));
			groupList.Add(group);
			
			group = new FormatExampleGroup("Predefined Formats");
			group.FormatExampleList.Add(new FormatExample("c -or- C", value.ToString("c")));
	        group.FormatExampleList.Add(new FormatExample("d -or- D (int only)", ((int)value).ToString("d")));
	        group.FormatExampleList.Add(new FormatExample("d2 -or- D2 (int only)", ((int)value).ToString("d2")));
	        group.FormatExampleList.Add(new FormatExample("d6 -or- D6 (int only)", ((int)value).ToString("d6")));
	        group.FormatExampleList.Add(new FormatExample("e", value.ToString("e")));
	        group.FormatExampleList.Add(new FormatExample("e2", value.ToString("e2")));
	        group.FormatExampleList.Add(new FormatExample("e6", value.ToString("e6")));
	        group.FormatExampleList.Add(new FormatExample("E", value.ToString("E")));
	        group.FormatExampleList.Add(new FormatExample("E2", value.ToString("E2")));
	        group.FormatExampleList.Add(new FormatExample("E6", value.ToString("E6")));
	        group.FormatExampleList.Add(new FormatExample("f -or- F", value.ToString("f")));
	        group.FormatExampleList.Add(new FormatExample("f2 -or- F2", value.ToString("f2")));
	        group.FormatExampleList.Add(new FormatExample("f6 -or- F6", value.ToString("f6")));
	        group.FormatExampleList.Add(new FormatExample("g -or- G", value.ToString("g")));
	        group.FormatExampleList.Add(new FormatExample("n -or- N", value.ToString("n")));
	        group.FormatExampleList.Add(new FormatExample("n2 -or- N2", value.ToString("n2")));
	        group.FormatExampleList.Add(new FormatExample("n6 -or- N6", value.ToString("n6")));
	        group.FormatExampleList.Add(new FormatExample("p -or- P", value.ToString("p")));
	        group.FormatExampleList.Add(new FormatExample("p2 -or- P2", value.ToString("p2")));
	        group.FormatExampleList.Add(new FormatExample("p6 -or- P6", value.ToString("p6")));
			group.FormatExampleList.Add(new FormatExample("r -or- R", value.ToString("R")));
	        group.FormatExampleList.Add(new FormatExample("x (int only)", ((int)value).ToString("x")));
	        group.FormatExampleList.Add(new FormatExample("x2 (int only)", ((int)value).ToString("x2")));
	        group.FormatExampleList.Add(new FormatExample("x6 (int only)", ((int)value).ToString("x6")));
	        group.FormatExampleList.Add(new FormatExample("X (int only)", ((int)value).ToString("X")));
	        group.FormatExampleList.Add(new FormatExample("X2 (int only)", ((int)value).ToString("X2")));
	        group.FormatExampleList.Add(new FormatExample("X6 (int only)", ((int)value).ToString("X6")));
			groupList.Add(group);
			
			group = new FormatExampleGroup("Custom Format Strings");
			group.FormatExampleList.Add(new FormatExample("0.00", value.ToString("0.00")));
	        group.FormatExampleList.Add(new FormatExample("#.00", value.ToString("#.00")));
	        group.FormatExampleList.Add(new FormatExample("#,###.##", value.ToString("#,###.##")));
	        group.FormatExampleList.Add(new FormatExample("0,000,000.00", value.ToString("0,000,000.00")));
	        group.FormatExampleList.Add(new FormatExample("#,###,###.00", value.ToString("#,###,###.00")));
	        group.FormatExampleList.Add(new FormatExample("(#,###.##)", value.ToString("(#,###.##)")));
	        group.FormatExampleList.Add(new FormatExample("0.00%", value.ToString("0.00%")));
	        group.FormatExampleList.Add(new FormatExample("#.##;(#.##);--", value.ToString("#.##;(#.##);--")));
			groupList.Add(group);
			
			return groupList;
		}
	}
}
