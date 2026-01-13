// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#include "SubSystem/CFSoundManagerSubsystem.h"
#include "Atom/AtomComponent.h"
#include "Atom/AtomSoundBase.h"
#include "Atom/AtomGameplayStatics.h"
#include "Kismet/GameplayStatics.h"
#include "Components/AudioComponent.h"
#include "Settings/CFSoundDataSettings.h"
#include "Data/CFSoundData.h"

void UCFSoundManagerSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Super::Initialize(Collection);

	UE_LOG(LogTemp, Log, TEXT("SoundManager: Initialize Started"));

	const UCFSoundDataSettings* Settings = GetDefault<UCFSoundDataSettings>();

	if (Settings == nullptr)
	{
		UE_LOG(LogTemp, Error, TEXT("SoundManager Settings not found. Check Project Settings."));
		return;
	}

	// ソフトポインタからパス文字列を取得
	FSoftObjectPath AssetPath = Settings->SoundDataAssetToLoad.ToSoftObjectPath();
	FString PathString = AssetPath.ToString();

	UE_LOG(LogTemp, Log, TEXT("SoundManager: Trying to load path: %s"), *PathString);

	if (PathString.IsEmpty())
	{
		UE_LOG(LogTemp, Error, TEXT("SoundDataAsset Path is Empty. Please set it in Project Settings."));
		return;
	}

	// ★変更点: LoadSynchronous ではなく StaticLoadObject を使用して強力にロード
	UObject* LoadedObject = StaticLoadObject(UObject::StaticClass(), nullptr, *PathString);
	UCFSoundData* SoundData = nullptr;

	if (LoadedObject)
	{
		// ロード成功。キャストして型を確認
		SoundData = Cast<UCFSoundData>(LoadedObject);

		if (SoundData == nullptr)
		{
			// ロードはできたが、クラスが違う場合
			UE_LOG(LogTemp, Error, TEXT("SoundManager: Loaded object is NOT UCFSoundData! It is: %s"), *LoadedObject->GetClass()->GetName());
			return;
		}
	}
	else
	{
		// ファイル自体が見つからない場合
		UE_LOG(LogTemp, Error, TEXT("SoundManager: StaticLoadObject FAILED. The asset is missing from the package. Path: %s"), *PathString);
		return;
	}

	UE_LOG(LogTemp, Log, TEXT("SoundManager: DataAsset Loaded Successfully!"));

	// マップの構築 (ここは元のまま)
	BGMMap.Empty();
	for (const FCFBGMBinding& Binding : SoundData->BGMBindings)
	{
		if (Binding.Sound)
		{
			BGMMap.Add(Binding.BGMType, Binding.Sound);
		}
	}

	SEMap.Empty();
	for (const FCFSEBinding& Binding : SoundData->SEBindings)
	{
		if (Binding.Sound)
		{
			SEMap.Add(Binding.SEType, Binding.Sound);
		}
	}

	UE_LOG(LogTemp, Log, TEXT("SoundManager: Registered %d SEs"), SEMap.Num());

	FWorldDelegates::OnPostWorldInitialization.AddUObject(this, &UCFSoundManagerSubsystem::OnWorldInit);
}

void UCFSoundManagerSubsystem::Deinitialize()
{
	FWorldDelegates::OnPostWorldInitialization.RemoveAll(this);

	if (BGMAtomComponent)
	{
		BGMAtomComponent->Stop();
		BGMAtomComponent->DestroyComponent();
		BGMAtomComponent = nullptr;
	}

	Super::Deinitialize();
}

void UCFSoundManagerSubsystem::OnWorldInit(UWorld* World, const UWorld::InitializationValues IVS)
{
	if (World && World->IsGameWorld())
	{
		if (BGMAtomComponent)
		{
			BGMAtomComponent->Stop();
			BGMAtomComponent->DestroyComponent();
		}
		// OuterをGameInstanceに設定することでレベル遷移で破壊されないようにする
		BGMAtomComponent = NewObject<UAtomComponent>(GetGameInstance());

		BGMAtomComponent->RegisterComponentWithWorld(World);
	}
}

void UCFSoundManagerSubsystem::CallBGM(CFBGMType BGMType)
{
	TObjectPtr<UAtomSoundBase> SoundToPlay = BGMMap.FindRef(BGMType);

	if (SoundToPlay && BGMAtomComponent)
	{
		BGMAtomComponent->SetSound(SoundToPlay);
		BGMAtomComponent->Play();
	}
	else if (SoundToPlay == nullptr)
	{
		// 登録されていない場合のみ警告を出す
		UE_LOG(LogTemp, Warning, TEXT("BGMType is not registered or Sound is null"));
	}
}

void UCFSoundManagerSubsystem::CallSE(CFSEType SEType)
{
	TObjectPtr<UAtomSoundBase> SoundToPlay = SEMap.FindRef(SEType);

	if (SoundToPlay)
	{
		UAtomGameplayStatics::PlaySound2D(GetGameInstance()->GetWorld(), SoundToPlay);
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("SEType is not registered"));
	}
}