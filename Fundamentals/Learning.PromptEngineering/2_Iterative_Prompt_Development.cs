using System.Runtime.CompilerServices;
using Azure.AI.OpenAI;

namespace Learning.PromptEngineering;

internal class _2_Iterative_Prompt_Development
{
    // こちらから引用させて頂きました♪
    // https://pg.asrock.com/Graphics-Card/AMD/Radeon%20RX%207900%20XTX%20Phantom%20Gaming%2024GB%20OC/index.jp.asp
    const string factSheet = """
            ## PRODUCT NAME:

            AMD Radeon RX 7900 XTX Phantom Gaming 24GB OC

            ## PRODUCT BRIEF

            - クロック：GPU /メモリ
              - ブーストクロック: 最大 2615MHz / 20Gbps
              - ゲームクロック: 2455MHz / 20Gbps
            - 主な仕様
              - AMD Radeon™ RX 7900 XTX GPU
              - 24GB GDDR6 on 384-Bit Memory Bus
              - 96 AMD RDNA 3 Compute Units (With Rt+Ai Accelerators)
              - 96MB AMD Infinity Cache™ Technology
              - PCI® Express 4.0 Support
              - 3 x 8 ピン 電源コネクタ
              - 3 x DisplayPort™ 2.1 / 1 x HDMI™ 2.1
            - 主な特徴
              - Polychrome SYNC
              - Phantom Gaming 3X Cooling System
              - Striped Ring Fan
              - Reinforced Metal Frame
              - スタイリッシュなメタルバックプレート
              - 0dB サイレントクーリング
              - Super Alloy Graphics Card

            ## PRODUCT FEATURES

            - Reverse Spin
              - The center fan spins reversely to lower turbulence and enhance air dispersion through the heatsink.
            - Phantom Gaming 3X Cooling System For Cool And Silent.
              - Crafted for the best balance between the thermal efficiency and silence by all the details.
            - Striped Ring Fan Designed For Enhanced Airflow
              - ASRock’s all new Striped Ring Fan get more lateral intake and to provide better airflow through the cooling array.
            - Air Deflecting Fin Guide Airflow To Go Through Regularly & Quickly.
              - Guide the airflow to go more regularly and quickly to enhance the cooling efficiency by the V-shaped cutting fins and the V-shaped air vents.
            - Ultra-Fit Heatpipe Consolidated To Maximize The Contact.
              - The heatpipes are consolidated to maximize the contact among each others and also the GPU baseplate for the optimized heat dissipation.
            - Nickel-Plated Copper Base Maximize GPU Contact Area.
              - With premium copper base heatsink design, the direct contact area to the GPU would be maximized to improve thermal transfer effectively.
            - High-Density Metal Welding Improve Heat Dissipation.
              - Effectively isolate all coverage of the gap between pipe and stacked fins, hence improve heat dissipation effectively.
            - Nano Thermal Paste Perfect Thermal Teamwork.
              - Eliminate the gaps in the contact area to maximize heat transfer and thermal efficiency.
            - Premium Thermal Pad Better Heat Transfer.
              - The premium thermal pad helps to transfer the heat of the components to the heatsink, improving heat dissipation.
            - Precise Screw Torque Optimized Mounting Pressure.
              - ASRock adopts precise screw torque when assembling its graphics cards to optimize the cooler mounting pressure to improve thermal efficiency while avoiding damage to GPU die.
            - 0dB サイレントクーリング
              - 作業負荷が軽いときには、0dB サイレントクーリング技術がノイズなしにファンを完全に停止します。
            - Polychrome SYNC
              - 統合された ARGB LED を使用して、独自のカラフルな照明効果を作成してください。また、Polychrome SYNC に対応する ASRock マザーボードと同期させることもできます。
            - LED On/Off Switch On Or Off? Switch As You Want.
              - You can switch not only the built-in ARGB LEDs but also the ARGB LEDs equipping with the connected strips/devices at once.
            - ARGB Pin-Header More ARGB, More Fancy.
              - With the ARGB pin-header, it can connect ARGB strips/devices to make the lighting effects more rich and vivid.
            - 強化された金属製フレーム
              - 強化された構造で PCB が曲がることを防止します。
            - スタイリッシュなメタルバックプレート
              - Phantom Gaming ルックです。Phantom Gaming グラフィックスカードのスタイリッシュなメタルプレートは、おしゃれな外観で頑丈な構造です。さらに、裏側のサーマルパッドも熱効率を向上させます。
            - Japanese SP-Cap
              - With a high conductive polymer as the electrolyte, it has a lower equivalent series resistance (ESR) to provide outstanding electrical characteristics. In addition to that, it also has excellence in product operational life, reliability and heat resistance.
            - SPS (Smart Power Stage)
              - Dr.MOS design features the latest SPS (Smart Power Stage) technology. It’s optimized for monitoring current and temperature of each phase, thus delivering smoother and neater power to the GPU with enhanced performance and OC capability.
            - プレミアム 100A パワーチョーク
              - 従来のチョークと比較して、ASRock の新世代プレミアムパワーチョークは飽和電流を最大 3 倍まで効果的に増加させるため、マザーボードの Vcore 電圧が強化・向上します。
            - 2 オンスの銅 PCB
              - PCB レイヤー向けに厳選された銅材料を使用しています。2 オンスの銅 PCB は、オーバークロックの際に温度を低く抑えて、優れたエネルギー効率を提供します。
            - マットブラック PCB
              - 新しい神秘的なマットブラックと銅のカラースキームは ASRock のハイエンドマザーボードの最高のコンポーネントにぴったりです。
            - 高密度ガラス繊維 PCB
              - 高密度ガラス繊維 PCB 設計は、PCB レイヤー間の隙間を減らして、湿度による電気短絡からマザーボードを保護します。
            """;
    internal static async Task RunAsync()
    {
        var options = OpenAIOptions.ReadFromUserSecrets();
        var client = Helper.CreateClient(options);

        await Zero(client, options);
        await First(client, options);
        await Second(client, options);
        await Third(client, options);
    }

    private static async Task Zero(OpenAIClient client, OpenAIOptions options)
    {
        var prompt = $"""
            あなたのタスクは、製品の Fact sheet に基づいて、製品の販売 Web サイトでの説明文を作成することです。
            説明文は、日本語で作成する必要があります。
            以下の3つのバッククォートで区切られた Fact sheet の情報に基づいて、製品の概要を記述してください。

            Fact sheet: ```{factSheet}```
            """;

        var result = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(result);
    }

    private static async Task First(OpenAIClient client, OpenAIOptions options)
    {
        var prompt = $"""
            あなたのタスクは、製品の Fact sheet に基づいて、製品の販売 Web サイトでの説明文を作成することです。
            説明文は、日本語で作成する必要があります。
            説明文の文字数は、100文字以内で作成してください。

            以下の3つのバッククォートで区切られた Fact sheet の情報に基づいて、製品の概要を記述してください。

            Fact sheet: ```{factSheet}```
            """;

        var result = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(result);

    }

    private static async Task Second(OpenAIClient client, OpenAIOptions options)
    {
        var prompt = $"""
            あなたのタスクは、製品の Fact sheet に基づいて、製品の販売 Web サイトでの説明文を作成することです。
            説明文は、日本語で作成する必要があります。
            
            この説明は、PCパーツの小売業者を対象としています。そのため、製品の技術的な強みにフォーカスしている必要があります。
            
            Fact sheet: ```{factSheet}```
            """;

        var result = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(result);
    }

    private static async Task Third(OpenAIClient client, OpenAIOptions options)
    {
        var prompt = $"""
            あなたのタスクは、製品の Fact sheet に基づいて、製品の販売 Web サイトでの説明文を作成することです。
            説明文は、日本語で作成する必要があります。
            この説明文は、PCパーツの小売業者を対象としています。そのため、製品の技術的な強みにフォーカスしてください。
            文章の構成として、"製品概要" で1文章で完結に概要を説明し、
            そのあとに "PRODUCT BRIEF" を Markdown の Table で出力してください。
            
            Fact sheet: ```{factSheet}```
            """;

        var result = await GetChatCompletionAsync(client, options, prompt);
        Console.WriteLine(result);
    }


    private static async Task<string> GetChatCompletionAsync(OpenAIClient client, OpenAIOptions options, string prompt)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            MaxTokens = 4000,
            Messages =
            {
                new ChatMessage(ChatRole.User, prompt)
            }
        };

        var response = await client.GetChatCompletionsAsync(options.DeploymentName, chatCompletionsOptions);
        return response.Value.Choices[0].Message.Content;
    }
}