// See https://aka.ms/new-console-template for more information

using Parser;

var test = @"

SELECT *, col, tbl.col, col2 as jason, tbl.col3 as wall FROM sch.""table"" as tbl;
SELECT *, col, tbl.col FROM sch.""table"" as tbl;

";

var reader = new StringReader(test);
using var tokenizer = new Tokenizer(reader);
var tokens = tokenizer.Scan();
var statements = Statement.Parse(tokens);
Console.WriteLine();