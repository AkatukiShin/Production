// Fill out your copyright notice in the Description page of Project Settings.

#include "Crane/Component/CFCommandComponent.h"
#include "CFCommandList.h"
#include "Crane/CFCraneBase.h"

// Sets default values for this component's properties
UCFCommandComponent::UCFCommandComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}

void UCFCommandComponent::Initialize(float CommandGraceTimeValue, float DeadZoneValue)
{
	CommandGraceTime = CommandGraceTimeValue;
	DeadZone = DeadZoneValue;
}


// Called when the game starts
void UCFCommandComponent::BeginPlay()
{
	PlayerOwner = Cast<ACFCraneBase>(GetOwner());

	if (!PlayerOwner)
	{
		UE_LOG(LogTemp, Error, TEXT("CFMoveComponentがNULLです。Pawn以外のアクターにアタッチされていませんか？"));
		return;
	}
}


// Called every frame
void UCFCommandComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UCFCommandComponent::InputCommand(const FInputActionValue& Value)
{
	const int32 MaxHistorySize = 100;
	if (InputLog.Num() > MaxHistorySize)
	{
		InputLog.RemoveAt(0);
	}
	FVector2D InputValue = Value.Get<FVector2D>();

	if (abs(InputValue.X) <= DeadZone && abs(InputValue.Y) <= DeadZone)
	{
		return;
	}

	// 入力値を元に角度を計測
	float Angle = FMath::Atan2(InputValue.Y, InputValue.X) * 180 / PI;
	if (Angle < 0.0f)	Angle += 360.0f;

	// 右
	if (Angle >= 337.5f || Angle < 22.5f)
	{
		//UE_LOG(LogTemp, Log, TEXT("Right Second"));
		InputLog.Add({ ECFInputDirection::Right, GetWorld()->GetDeltaSeconds() });
		CommandCheck();
	}
	// 右上
	else if (Angle < 67.5f)
	{
		//UE_LOG(LogTemp, Log, TEXT("RightUp"));
		InputLog.Add({ ECFInputDirection::RightUp, GetWorld()->GetDeltaSeconds() });
	}
	// 上
	else if (Angle < 112.5f)
	{
		//UE_LOG(LogTemp, Log, TEXT("Up"));
		InputLog.Add({ ECFInputDirection::Up, GetWorld()->GetDeltaSeconds() });
	}
	// 左上
	else if (Angle < 157.5f)
	{
		//UE_LOG(LogTemp, Log, TEXT("LeftUp"));
		InputLog.Add({ ECFInputDirection::LeftUp, GetWorld()->GetDeltaSeconds() });
	}
	// 左
	else if (Angle < 202.5f)
	{
		//UE_LOG(LogTemp, Log, TEXT("Left"));
		InputLog.Add({ ECFInputDirection::Left, GetWorld()->GetDeltaSeconds() });
		CommandCheck();
	}
	// 左下
	else if (Angle < 247.5f)
	{
		//UE_LOG(LogTemp, Log, TEXT("LeftDown"));
		InputLog.Add({ ECFInputDirection::LeftDown, GetWorld()->GetDeltaSeconds() });
	}
	//　下
	else if (Angle < 292.5f)
	{
		//UE_LOG(LogTemp, Log, TEXT("Down"));
		InputLog.Add({ ECFInputDirection::Down, GetWorld()->GetDeltaSeconds() });
	}
	// 右下
	else
	{
		//UE_LOG(LogTemp, Log, TEXT("RightDown"));
		InputLog.Add({ ECFInputDirection::RightDown, GetWorld()->GetDeltaSeconds() });
	}
}

void UCFCommandComponent::CommandCheck()
{
	float time = 0;

	// Playerが無効、または既にコマンドアニメーション再生中の場合は、新しいコマンドを受け付けない
	//if (!Player || Player->IsCommandAnimationPlaying())
	//{
	//	return;
	//}

	//if (Player->GetPlayerType() == ECFCraneActionType::Attack)	return;

	//入力履歴がなければリターン
	if (InputLog.Num() == 0)	return;
	CommandCheckLog.Empty();

	//指定秒数遡る
	for (int i = InputLog.Num() - 1; i >= 0; --i)
	{
		if (time >= CommandGraceTime)	break;
		CommandCheckLog.Add(InputLog[i].Direction);
		time += InputLog[i].Duration;
	}

	CFCommandList Cmds;

	// コマンドが成立しているか確認する
	if (CommandSuccesCheck(CommandCheckLog, Cmds.RightRotateAttack_N))
	{
		UE_LOG(LogTemp, Log, TEXT("RightRotateAttack"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(ECFAttackType::RightRotate);
		return;
	}
	if (CommandSuccesCheck(CommandCheckLog, Cmds.RightRotateAttack_R))
	{
		UE_LOG(LogTemp, Log, TEXT("RightRotateAttack"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(ECFAttackType::RightRotate);
		return;
	}
	if (CommandSuccesCheck(CommandCheckLog, Cmds.LeftRotateAttack_N))
	{
		UE_LOG(LogTemp, Log, TEXT("LeftRotateAttack"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(ECFAttackType::LeftRotate);
		return;
	}
	if (CommandSuccesCheck(CommandCheckLog, Cmds.LeftRotateAttack_R))
	{
		UE_LOG(LogTemp, Log, TEXT("LeftRotateAttack"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(ECFAttackType::LeftRotate);
		return;
	}
	if (CommandSuccesCheck(CommandCheckLog, Cmds.RightUpper))
	{
		UE_LOG(LogTemp, Log, TEXT("RightUpper"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(ECFAttackType::RightUpper);
		return;
	}
	if (CommandSuccesCheck(CommandCheckLog, Cmds.LeftUpper))
	{
		UE_LOG(LogTemp, Log, TEXT("LeftUppder"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(ECFAttackType::LeftUpper);
		return;
	}
	/*if (CommandSuccesCheck(CommandCheckLog, Cmds.FrontUpper))
	{
		UE_LOG(LogTemp, Log, TEXT("FrontUppder"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(FName("FrontUpper"));
		return;
	}
	if (CommandSuccesCheck(CommandCheckLog, Cmds.BackUpper))
	{
		UE_LOG(LogTemp, Log, TEXT("BackUppder"));
		CommandCheckLog.Empty();
		InputLog.Empty();
		PlayerOwner->HandleCommand(FName("BackUpper"));
		return;
	}*/
	return;
}

bool UCFCommandComponent::CommandSuccesCheck(const TArray<ECFInputDirection>& CommandLog, const TArray<ECFInputDirection>& CommandList) const
{
	int CommandLogNum = CommandLog.Num() - 1;
	int CommandListNum = CommandList.Num() - 1;
	int j = 0;
	for (int i = 0; i < CommandLogNum; ++i)
	{
		if (CommandLog[i] == CommandList[j])
		{
			j++;
			if (j > CommandListNum)	return true;
		}
	}

	return false;
}

