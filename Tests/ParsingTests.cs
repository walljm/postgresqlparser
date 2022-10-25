using Parser;

namespace Tests
{
    [TestClass]
    public class ParsingTests
    {
        [DataTestMethod()]
        public void Sql1Test()
        {
            var test = @"

SELECT distinct *, col, tbl.col, col2 as jason, tbl.col3 as wall FROM sch.""table"" as tbl where col1 > col4 or col5 group by col1, tbl.col2 order by col2 desc, col1;

";

            var reader = new StringReader(test);
            using var tokenizer = new Tokenizer(reader);
            var tokens = tokenizer.Scan();
            var statements = Statement.Parse(tokens);
            Assert.AreEqual(@"SELECT DISTINCT
     *
    ,col
    ,tbl.col
    ,col2     AS jason
    ,tbl.col3 AS wall
FROM
    sch.""table"" AS tbl
WHERE
    col1 > col4
    OR col5  
GROUP BY
     col1
    ,tbl.col2
ORDER BY
     col2 DESC
    ,col1 ASC
"
, statements.First().Print(4, 0));
        }

        [DataTestMethod()]
        public void Sql2Test()
        {
            var test = @"

SELECT distinct on (col1, col2) *, col, tbl.col FROM sch.""table"" as tbl limit 50 offset 300;

";

            var reader = new StringReader(test);
            using var tokenizer = new Tokenizer(reader);
            var tokens = tokenizer.Scan();
            var statements = Statement.Parse(tokens);
            Assert.AreEqual(@"SELECT DISTINCT ON (
         col1
        ,col2
    )
     *
    ,col
    ,tbl.col
FROM
    sch.""table"" AS tbl
OFFSET 300
LIMIT 50
", statements.First().Print(4, 0));
        }
    }
}