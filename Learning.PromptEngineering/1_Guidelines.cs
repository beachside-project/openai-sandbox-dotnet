using Azure.AI.OpenAI;

namespace Learning.PromptEngineering;

internal class _1_Guidelines
{
    internal static async Task RunAsync()
    {
        var options = OpenAIOptions.ReadFromUserSecrets();
        var client = Helper.CreateClient(options);

        //await Tactic1_1And1_2(client, options);
        //await Tactic1_3(client, options);
        //await Tactic1_4(client, options);
        //await Tactic2_1(client, options);
        await Tactic2_2(client, options);
    }

    private static async Task Tactic1_1And1_2(OpenAIClient client, OpenAIOptions options)
    {
        // Principle 1 - Tactic 2: delimiter を使う / 構造化された出力を指示する

        
        //var prompt = """
        //    架空の映画のタイトルと監督・主演とジャンルのリストを3つを生成してください。
        //    回答はJSON形式で回答してください。
        //    """;

        //var prompt = """
        //    架空の映画のタイトルと監督・主演とジャンルのリストを3つを生成してください。
        //    日本語で回答してください。
        //    回答は、JSON形式でJSON フォーマットのキーは以下を使います。

        //    title, director, mainActor, genre
        //    """;

        var prompt = """
            あなたのタスクは、架空の映画のタイトルとその監督、主演とジャンルを作ることです。
            日本語で回答でします。
            3つの映画の回答を生成します。
           
            回答は、"回答例" のタグにを参考にJSONフォーマットの配列で出力してください。

            <回答例>
            [
             { "title": "1つめの映画のタイトル", "director": "1つめの映画の監督" "actor", "genre":"1つめの映画のジャンル" },
             { "title": "2つめの映画のタイトル", "director": "2つめの映画の監督" "actor", "genre":"2つめの映画のジャンル" },
             { "title": "3つめの映画のタイトル", "director": "3つめの映画の監督" "actor", "genre":"3つめの映画のジャンル" }
            ]
            </回答例>
            """;

        var prompt2Response = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(prompt2Response);
    }

    private static async Task Tactic1_3(OpenAIClient client, OpenAIOptions options)
    {
        // Tactic 3: 条件が満たされているかを確認する

        //var text = @"こんにちは。今日はいい天気ですね。
        //             夜は雨が振るので外出するときは傘を持っていきましょう。";

        var text = """
            おいしい抹茶のいれかたは、まずふるった抹茶を茶杓2杯分（ 約2グラム ）茶碗に入れます。
            次に、70から80度に冷ましたお湯を70ミリリットル準備し、そこから少量を茶碗に注ぎ、ダマができないように、茶せんで溶くようにして混ぜます。
            残りのお湯を注いで、手首を前後に動かして最初は小刻みに、次は表面を整えるようにして混ぜます。大きな泡ができたら茶筅の先でつぶし、表面を整えるときれいに見えます。
            泡が細かいほど口当たりがなめらかでおいしく仕上がります。これで完成です。おいしいお菓子と一緒にゆっくり味わいましょう。
            """;

        var prompt = $"""
            ### で区切られたテキストが提供されます。一連の手順が含まれている場合、その手順を次の形式で書き直します。

            Step 1: ...
            Step 2: ...
            ...
            Step N: ...

            もし、テキストに手順がふくまれていない場合は、「手順は含まれていません。」と回答します。

            ### {text} ###
            """;

        var prompt2Response = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(prompt2Response);
    }

    private static async Task Tactic1_4(OpenAIClient client, OpenAIOptions options)
    {
        var question = "太陽系で一番大きい惑星は何ですか。";

        var prompt = $"""
            あなたのタスクは、一貫したスタイルで回答することです。

            <User>: 北海道は大きいですか
            <Assistant>: でっかいどー

            <User>: 太陽の大きさは
            <Assistant>: でっかいどー

            <User>: {question}
            """;

        var prompt2Response = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(prompt2Response);
    }

    private static async Task Tactic2_1(OpenAIClient client, OpenAIOptions options)
    {

        var text = """
            あるところにおじいさんとおばあさんが住んでいて、おじいさんは山に芝を刈りに行きおばあさんは川で洗濯をしに行っていると、川から大きな桃が流れてきます。早速大きな桃を食べるために持ち帰り中を割ってみると中から赤子が出てきたではありませんか。
            二人は桃から生まれた赤子に桃太郎と名付けます。桃太郎は急速に成長していき、ある時、とあるうわさを聞きつけます。なんでも、悪い鬼が人間から金銀財宝を巻き上げているという噂を。
            桃太郎は鬼の悪事が許せず、鬼を退治しに行くと育ててくれたお爺さんおばあさんに打ち明けました。するとおじいさんとおばあさんは、旅先で食べるようにと黍団子を手渡すのです。
            旅の道中犬に出会った桃太郎は、犬から黍団子が欲しいと打ち明けられ、その代わりに鬼退治で加勢してほしいと頼み犬は加勢に了承します。今度は猿に同じことを言われ猿も加勢に賛同し、最後にキジと出会い記事にも黍団子を私すべての黍団子を使い果たします。
            桃太郎は犬、猿、記事をお供に従えて鬼退治へと鬼が住む島に乗り込むのです。鬼ノ島に乗り込んだ桃太郎一行は鬼と対峙し、双方争いの中、桃太郎側に勝敗が傾くと鬼たちは降参の意向を示し始めますが果たしてどうなるのでしょうか。
            """;

        //var prompt = $"""
        //    あなたのタスクは、次のアクションを実行することです。
        //    1. 以下にある ### で区切られた TEXT を100文字程度の1文に要約します。
        //    2. 要約した文章を英語に翻訳します。
        //    3. 登場人物の名前を出力します。
        //    4. JSON Object を出力します。JSONのキーには "jp_summary" と "names" を使います。

        //    それぞれの回答は、改行で区切ります。

        //    TEXT:
        //    ###
        //    {text}
        //    ###
        //    """;

        var prompt = $"""
            あなたのタスクは、次のアクションを実行することです。

            1. 以下にある ### で区切られた TEXT を100文字程度の1文に要約します。
            2. 要約した文章を英語に翻訳します。
            3. 登場人物の名前を出力します。
            4. JSON Object を出力します。JSONのキーには "jp_summary" と "names" を使います。

            回答は次の形式で出力します。

            概要: <要約>
            翻訳: <要約の翻訳>
            登場人物: <登場人物のリスト>
            テキスト: <要約するテキスト>

            TEXT:
            ###
            {text}
            ###
            """;

        var prompt2Response = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(prompt2Response);

    }

    private static async Task Tactic2_2(OpenAIClient client, OpenAIOptions options)
    {
        // 英語に翻訳させた方が精度よかった。日本語のままだったら計算がグダグダになる。
        // 生徒の回答は意図的に間違えてる。
        var prompt = """
            あなたのタスクは、問題に対して生徒の回答が正しいかを判断することです。

            タスクを完了するのに以下の手順で行ないます。

            - 最初に、問題を英語に翻訳し、翻訳した文章を使ってあなたが正しい回答を作成してください。 
            - 次に、あなたの回答と生徒の回答を比較して、生徒の回答が正しいかを評価してください。

            自身で問題を解くまでは、生徒の回答が正しいか判断してはいけません。
            以下のフォーマットを使って返答してください。

            ## 問題

            <ここに問題が書かれます>

            ### 生徒の回答
           
            <ここに生徒の回答が書かれます>

            ## 評価

            ### 正しい回答

            <解決までの手順を step by step でここに書きます>
     
            ### 生徒の回答は正しいか

            <正解または不正解で回答>

            ## 問題
            
            太陽光発電の建設にあたり、費用は以下です。

            - 土地代は、30000円/平方メートル
            - ソーラーパネルのコストは、40000円/平方メートル
            - 年間の保守費用は基本料金として1000000円と、追加の保守費用として5000円/平方メートルがかかります。
            
            1年間のX平方メートルの合計コストは何円になるでしょうか。
           
            ### 生徒の回答
            
            それぞれのコストは
            - 土地代: 30000X
            - ソーラーパネルのコスト: 40000X
            - 保守費用: 5000X
            
            したがって合計は 30000X + 40000X + 1000000 + 5000X = 72000X + 1000000

            ## 評価
            """;

        var prompt2Response = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(prompt2Response);
    }
    private static async Task<string> GetChatCompletionAsync(OpenAIClient client, OpenAIOptions options, string prompt)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            MaxTokens = 1000,
            Messages =
            {
                new ChatMessage(ChatRole.User, prompt)
            }
        };

        var response = await client.GetChatCompletionsAsync(options.DeploymentName, chatCompletionsOptions);
        return response.Value.Choices[0].Message.Content;
    }
}







