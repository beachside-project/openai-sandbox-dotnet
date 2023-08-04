using Microsoft.SemanticKernel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace SemanticKernelSamples.CognitiveSearchMemorySample;

internal class ComicSearchSample
{
    private const string CognitiveSearchIndexName = "Kadokawa";

    internal static async Task RunAsync(IKernel kernel)
    {
        await UpsertIndexAsync(kernel);
        await SearchAsync(kernel);
    }

    private static async Task SearchAsync(IKernel kernel)
    {
        var questions = new[]
        {
            "教会関連のラブコメ", // 白聖女と黒牧師
            "原作が週刊少年マガジンのラブコメ", // 彼女、お借りします
            "原作がKissのサスペンス", // 「御手洗家、炎上する」→サスペンスってワードは text にはいってない。サスペンスを使ってるのは「なれの果ての僕ら」
            "いじめや復讐のマンガ", // 「御手洗家、炎上する」か「なれの果ての僕ら」
        };

        foreach (var question in questions)
        {
            Console.WriteLine($"Q: {question}\n");
            // 1st arg(collection) に cognitive search の index をセット
            var memories = kernel.Memory.SearchAsync(CognitiveSearchIndexName, question, limit: 2);
            await foreach (var memory in memories)
            {
                //Console.WriteLine("URL:     : " + memory.Metadata.Id);
                //Console.WriteLine("Title    : " + memory.Metadata.Description);
                //Console.WriteLine("Relevance: " + memory.Relevance);

                Console.WriteLine(JsonSerializer.Serialize(memory, options: new()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true,
                }));
                Console.WriteLine();
            }

            Console.WriteLine("\n ^^^^^^^^^^^^^^^^^^^ \n");
        }
    }

    private static async Task UpsertIndexAsync(IKernel kernel)
    {
        foreach (var comic in ComisData)
        {
            Console.WriteLine($"Token: {Helpers.GetTokenCount(comic.Description)}");


            //await kernel.Memory.SaveReferenceAsync(
            //    collection: CognitiveSearchIndexName,
            //    externalSourceName: CognitiveSearchIndexName,
            //    externalId: comic.Url,
            //    text: comic.Description,
            //    description: comic.Title);
        }
    }

    public class ComicIndex
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }

        public ComicIndex(string url, string title, string description)
        {
            Url = url;
            Title = title;
            Description = description;
        }
    }

    private static ComicIndex[] ComisData = {
            new("https://mangaip.kodansha.co.jp/news/detail/43399/", "御手洗家、炎上する", @"
病院を経営する裕福な一家・御手洗家が全焼するという不幸な炎上事件があった。その13年後、家事代行業の村田杏子（永野芽郁）は、新規顧客である御手洗家に向かう。
出迎えたのは美しく凄味のある御手洗家の後妻、真希子（鈴木京香）。無事採用され働くことになった杏子だが、彼女の御手洗家潜入には、ある目的があったーー。
13年前の大火事によって家族も家も奪われた村田杏子、旧姓・御手洗杏子。彼女の人生を狂わせたのは、御手洗家の後妻となり、主婦モデルやインフルエンサーとして裕福で華やかな日々を送っている真希子だと確信した杏子は、家政婦として御手洗家に潜入する。
自分たちの家族を崩壊させた真希子への復讐を果たすため、その証拠を掴もうと必死に奔走する杏子は徐々に真希子の信頼を得て彼女に取り入っていくのだが、そんな杏子の障害となる、自室に引きこもっていた、御手洗家の長男・希一、杏子の正体を嗅ぎまわる次男・真二、かつて自分たちを捨てた父・御手洗治らそれぞれ秘密を抱えた御手洗家の家族たち・・・。
そして、恐ろしいほどの凄みと妖艶さを纏う真希子が杏子の前に立ちふさがり、ついに杏子vs真希子の火花散る復讐劇が幕を開ける
原作は藤沢もやし『御手洗家、炎上する』（講談社「Kiss」刊）
            "),

            new("https://mangaip.kodansha.co.jp/news/detail/43400/", "白聖女と黒牧師", @"""
牧師は過保護に聖女を守り、聖女は「加護」で牧師を守る。この恋は、無自覚に育まれていく──。
とある教会。そこには可愛いけどだらけグセのある聖女さまと、真面目で過保護で料理上手な牧師さまが住んでいました。
穏やかな日々の中で、密かに恋する聖女と鈍感な牧師が繰り広げる、""無自覚いちゃラブコメ""。
もどかしい二人の関係が行きつく先は――!?
            "),

            new("https://mangaip.kodansha.co.jp/news/detail/43331/", "聖者無双～サラリーマン、異世界で生き残るために歩む道～", @"
原作：ブロッコリーライオン／漫画：秋風緋色／その他：sime『聖者無双』（講談社「水曜日のシリウス」連載）
出世を目前に凶弾に倒れた元サラリーマン、15歳の治癒士として異世界転生！　新しい人生目標は「寿命で老衰」すること！
元サラリーマンが異世界で無双!?
出世を目前に凶弾に倒れた一人のサラリーマン。
なぜか神様によって十五歳の治癒士・ルシエルとして異世界に転生することになってしまった。
しかも生まれ落ちたこの国では、治癒士は嫌われ者のようで......。
身の危険を感じたルシエルは、護身のため敢えて冒険者ギルドの門を叩く。
しかし、訓練は想像以上に厳しく、さらには「物体X」という謎の飲み物を飲まされる毎日。
あれ？　何だか治癒士とは関係ない生活のような......？
""ドＭ""で""ゾンビ""な""治癒士""の生き残りをかけた日常が始まる――！

原作：ブロッコリーライオン／漫画：秋風緋色／その他：sime『聖者無双』（講談社「水曜日のシリウス」連載）

            """),

            new("https://mangaip.kodansha.co.jp/news/detail/43348/", "彼女、お借りします", @"""
宮島礼吏による世界累計1000万部突破の大人気ラブコメ

ダメダメ大学生・木ノ下和也は清楚可憐な""レンタル彼女（レンカノ）""・水原千鶴と出会い、家族にも友人にも、千鶴が ""彼女""だと嘘をついてしまう。
本当のことが言い出せないまま日々をすごす和也の周囲には、謎アタックを仕掛けてくる、小悪魔的な元カノ・七海麻美、やや強引なところがある、超積極的な彼女(仮)・更科瑠夏、極度の人見知りだが、健気で頑張り屋の後輩レンカノ・桜沢 墨と、超絶美少女な""彼女""がいっぱい!!
たくさんの季節を一緒にすごし、様々なイベントを乗り越えるなかで、千鶴への想いを募らせていく和也は、女優として活躍したいという千鶴を応援し続けることを誓う。
しかし、そんな折に千鶴の祖母・小百合の体調が悪化。
出演映画を小百合に見せるという千鶴の夢が危ぶまれる事態に。
｢一緒に映画、作るんだよ!!｣
和也はクラウドファンディングで千鶴主演の映画を作ることを決意して......。
動き始めた、映画制作。
隣に引っ越してきた八重森みにを新たに巻き込み、たった一度の""レンタル""から動き出した和也の""リアル""が、より輝きを増していく！
和也と千鶴の映画作りは、はたしてどんな結末を迎えるのか──!?

原作: 宮島礼吏『彼女、お借りします』（講談社「週刊少年マガジン」連載）
            """),

            new("https://mangaip.kodansha.co.jp/news/detail/43311/", "実は俺、最強でした？", @"
赤ちゃんに転生したのに捨てられてしまうが、チート魔法力で生き延びる！
ヒキニートがある日突然、異世界の王子様に転生した――と思ったら、直後に最弱認定され命がピンチに!?　万能魔法で理想の引きこもりライフを目指す成り上がりストーリー！
「チート能力を授けて、異世界に転生させます！　第二の人生を謳歌してね!?」
突如女神にこう告げられ、所謂引きニートだった俺は、異世界の王子の赤ちゃんとして目覚めたのだった。
魔力が絶対的なこの世界、圧倒的な魔力と王族の地位を得た俺なら「悠々自適の引きこもりライフが満喫できるじゃないか！」と思った瞬間。あっさりと、遠くの森に捨てられた...。
いきなりの大ピンチを迎えた俺の目の前に現れたのは、とある勘違いから俺の従者になるフレイム・フェンリルの「フレイ」と、辺境伯の「ゴルド・ゼンフィス」だった。
それから9年、拾われたゼンフィス家で何不自由なく育った俺は、可愛い妹「シャル」をはじめ、前世では考えもしなかった家族というものに囲まれて、再び引き籠もりライフのための計画を図るのだが...。
最強!?の俺に、平穏な生活は果たしていつ訪れるのでしょうか？

【原作】澄守彩『実は俺、最強でした？』（講談社「Kラノベブックス」刊）
            "),

            new("https://mangaip.kodansha.co.jp/news/detail/43307/", "なれの果ての僕ら", @"""
同窓会に参加したクラスメイトたちによる壮絶な監禁劇、極限状態の中で変貌していく人間の狂気を描いた話題作がついに実写化。
人の""善性""を問う悍ましい実験とは――
同窓会のために母校に集まったネズ（井上瑞稀）ら元6年2組の23人は、3日間監禁され、13人死亡すると言う大事件に巻き込まれた。監禁したのは、かつてのクラスメイトの夢崎みきお（犬飼貴丈）。
みきおの目的は、命の危機という極限状態で、人間の""善性""がどれだけ保てるのかを試すことだった。誰が誰を裏切り、誰が誰を殺すことになるのか...。
乱れゆく秩序の中で、人が取る行動とは...。
復讐、いじめ、裏切り、暴露など、予測不能な展開が続く衝撃のサスペンス！

原作:内海八重『なれの果ての僕ら』（講談社「週刊少年マガジンKC」刊）
            """),
            new("https://mangaip.kodansha.co.jp/news/detail/42614/", "女神のカフェテラス", @"
東大現役合格の秀才・粕壁隼。訃報を聞き実家に戻ると、そこには見知らぬ5人の美女が!?　亡き祖母が残したカフェテラスを舞台にくり広げられる、運命の人×5人との贅沢すぎる共同生活！
――どこかの海辺にある、古びた喫茶店。
そこには...女神様がいるらしい――
ケンカ別れした祖母が遺した喫茶店「Familia」。
「赤字だらけの店、さっさと畳んで駐車場に建て替えよう」と主人公、粕壁隼が3年ぶりに帰省をすると
そこには「おばあちゃんの家族」を語る見知らぬ5人のカワイイ女の子が!!
いきなり下着？　全裸!?　ちょっとまて、誰が空き巣だ！
ここはオレの家だ！ 絶対に追い出してやる！
最悪の出会いから、恋と家族の物語が始まるっ！
5人全員「正ヒロイン」の、夢のハーレム共同生活！
ヒロイン多すぎシーサイドラブコメ!!
原作は瀬尾公治『女神のカフェテラス』（講談社「週刊少年マガジン」連載）
            ")
        };
}