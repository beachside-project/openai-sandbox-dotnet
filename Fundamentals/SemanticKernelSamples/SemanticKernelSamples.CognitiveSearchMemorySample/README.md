# SemanticKernelSamples.CognitiveSearchMemorySample

SemanticKernel の Memory に AzureCognitiveSearch を利用すると

- id にはリンクの URL、text の値を embeddings して cognitive search へ登録しておくことで、検索したワードと登録されている embeddings に近いリンクを引いてくるって動作。

## 注意

7/19時点最新の v0.17.230718.1 は間違って NuGet package の名前を `Microsoft.SemanticKernel.Connectors.Memory.AzureSearch` と登録したっぽい。

今後、正しい `Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch` に戻るはず。


## kernel.Memory.SaveReferenceAsync の挙動

v0.17.230718.1 - preview 時点での SaveReferenceAsync 挙動メモ

- collection: CognitiveSearch の index 名になる。
- externalSourceName: cognitive search には空で登録されている。
- externalId: base64 encode した値が cognitive search の id として登録される。
- text: 実際にembeddings して cognitive search の Embedding field に登録される。text field も存在するが空で登録されているのでそれは意味がわからん。
  - https://github.com/microsoft/semantic-kernel/blob/f1fb50f5c96ca97f312d01fb77c0943a17c1c80c/dotnet/src/SemanticKernel/Memory/SemanticTextMemory.cs#L51
- description: cognitive search に登録されるので、text の値を返したいなら同じ値を、もしくは検索結果として返したいカスタムな値を登録する。
- additionalMetadata: 追加で検索結果で返したい値を登録する。型は `Edm.String`。要望として、`Collection(Edm.String)` にしてほしい。

## kernel.Memory.SaveReferenceAsync の挙動