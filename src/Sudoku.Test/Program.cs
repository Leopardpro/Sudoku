﻿using System.Linq;

int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

var selection =
	from x in arr
	orderby x ascending
	orderby x + 1 ascending
	orderby x * 45 + 17 & 12 descending
	select x;