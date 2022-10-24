// See https://aka.ms/new-console-template for more information

using Parser;

var test = @"

SELECT distinct *, col, tbl.col, col2 as jason, tbl.col3 as wall FROM sch.""table"" as tbl where col1 > col4 or col5 group by col1, tbl.col2 order by col2 desc, col1;
SELECT distinct on (col1, col2) *, col, tbl.col FROM sch.""table"" as tbl limit 50 offset 300;

";

var reader = new StringReader(test);
using var tokenizer = new Tokenizer(reader);
var tokens = tokenizer.Scan();
var statements = Statement.Parse(tokens);
foreach (var stmt in statements)
{
    Console.WriteLine(stmt.Print(4, 0));
    Console.WriteLine();
}