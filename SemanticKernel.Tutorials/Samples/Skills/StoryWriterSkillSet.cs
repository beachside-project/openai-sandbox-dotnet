using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace SemanticKernel.Tutorials.Samples.Skills;

public static class StoryWriterSkillSet
{
    public static IKernel AddStoryWriterSkillSet(this IKernel kernel)
    {
        kernel.CreateSemanticFunction("""
            あなたは Story writer チャットボットです。
            物語やストーリーを作成してほしいユーザーからの問い合わせに対して、与えられた文章に "タイトル" を付け、"序文" と "まとめ" の文章を作成するチャットボットです。
            文章作成以外の質問には回答してはいけません。

            出力例:
            ```
            # タイトルを記載

            ## 序文

            ここに序文を記載してください。

            ## 4つのトピック

            {{$input}}

            ## まとめ

            入力の要約をここに書いてください。
            ```

            文章:
            ```
            {{$input}}
            ```
            """,
    config: new PromptTemplateConfig
    {
        Input = new PromptTemplateConfig.InputConfig
        {
            Parameters =
            {
                        new() { Name = "input", Description = "文章"}
            },
        },
        Description = "文章に対してタイトルを付けて、まとめの章と序文を追加してください。",
        Completion = new PromptTemplateConfig.CompletionConfig
        {
            MaxTokens = 5000,
        },
    },
    "Summarize",
    "StoryWriterSkill");

        kernel.CreateSemanticFunction("""
            あなたは Story writer チャットボットです。
            与えられた Keyword を使って文章を作成してください。
            Keyword の区切り文字は "," です。
            Keyword が 4 つの場合には、4つのセクションを作成してください。
            セクションの前には、その keyword を見出しとして記載してください。

            Keywordが "item1,item2,item3,item4" の場合の出力例は以下です。

            出力例:
            ```
            ### item1

            item1 に関する文章を100文字程度で記載します。

            ### item2

            item2 に関する文章を100文字程度で記載します。

            ### item3

            item3 に関する文章を2から3段落で記載します。

            ### item4

            item4 に関する文章を2から3段落で記載します。
            ```

            Keyword:
            ```
            {{$input}}
            ```
            """,
            config: new PromptTemplateConfig
            {
                Input = new PromptTemplateConfig.InputConfig
                {
                    Parameters =
                    {
                        new()
                        {
                            Name = "input",
                            Description = "カンマ区切りのキーワード",
                            DefaultValue = null
                        }
                    }
                },
                Description = "与えられた keyword から文章を作成します。",
                Completion = new PromptTemplateConfig.CompletionConfig
                {
                    MaxTokens = 2000,
                }
            },
            "Write",
            "StoryWriterSkill");

        kernel.CreateSemanticFunction(""""
            あなたは Story writer チャットボットです。

            与えられた topic から関連性の強いキーワードを4つ回答してください。
            キーワードは "," で区切り、1行で回答してください。

            出力例:
            入力が "リンゴ" の場合の出力例は以下のようになります。
            ```
            ビタミンC,果物,リンゴ酸,青森
            ```

            トピック: {{$input}}
            """",
            config: new PromptTemplateConfig()
            {
                Input = new PromptTemplateConfig.InputConfig()
                {
                    Parameters =
                    {
                        new PromptTemplateConfig.InputParameter()
                        {
                            Name = "input",
                            Description = "トピックを作るための単語・文章"
                        },
                    }
                },
                Description = "入力された単語・文章から4つのキーワードを生成します。",
            },
            "CreateKeywords",
            "StoryWriterSkill");

        return kernel;
    }
}