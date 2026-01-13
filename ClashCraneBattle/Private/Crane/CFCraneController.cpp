// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#include "Crane/CFCraneController.h"
#include "Components/InputComponent.h"
#include "Crane/CFPlayerInputData.h"
#include "Crane/CFCraneData.h"
#include "Crane/CFPlayer.h"
#include "Crane/CFCraneActionType.h"
#include "CFInputDirection.h"
#include "CFInputHistory.h"
#include "CFCommandList.h"
#include "Crane/Component/CFCommandComponent.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "Interface/CFIOnPrizeAcquired.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetMaterialLibrary.h"

void ACFCraneController::BeginPlay()
{
	Super::BeginPlay();
	// Hp
	CHp = CraneDatas->Hp;
	// Move
	CMoveSpeed = CraneDatas->MoveSpeed;
	CUpSpeed = CraneDatas->UpSpeed;
	CDownSpeed = CraneDatas->DownSpeed;
	// Attack
	CCommandGraceTime = CraneDatas->CommandGraceTime;
	CUpperAttackValue = CraneDatas->UpperAttackValue;
	CRotationAttackValue = CraneDatas->RotationAttackValue;
	CGrabAttackValue = CraneDatas->GrabAttackValue;
	// Kakin
	CMoney = CraneDatas->Money;
	CUpperSuccesMoney = CraneDatas->UpperSuccesMoney;
	CRotationSuccesMoney = CraneDatas->RotationSuccesValue;
	// Recovery
	CRecoveryValue = CraneDatas->RecoveryValue;

	PrimaryActorTick.bCanEverTick = false;
}

ACFCraneController::ACFCraneController()
{
}

void ACFCraneController::Tick(float DeltaSeconds)
{

}
/// <summary>
///	IMCを変更する関数
/// </summary>
/// <param name="Type">Type状態のIMCを設定する</param>
void ACFCraneController::ChangeIMC(ECFCraneActionType Type)
{
	switch (Type)
	{
	case ECFCraneActionType::Idle:
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(this->GetLocalPlayer()))
		{
			Subsystem->RemoveMappingContext(StanMappingContext);
			Subsystem->AddMappingContext(CraneMappingContext, 0);
		}
		break;
	case ECFCraneActionType::Stan:
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(this->GetLocalPlayer()))
		{
			Subsystem->RemoveMappingContext(CraneMappingContext);
			Subsystem->AddMappingContext(StanMappingContext, 0);
		}
		break;
	case ECFCraneActionType::GrabEachOther:
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(this->GetLocalPlayer()))
		{
			Subsystem->RemoveMappingContext(CraneMappingContext);
			Subsystem->AddMappingContext(GrabEachOtherMappingContext, 0);
		}
		break;
	case ECFCraneActionType::RoundEnd:
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(this->GetLocalPlayer()))
		{
			Subsystem->RemoveMappingContext(CraneMappingContext);
			Subsystem->AddMappingContext(RoundEndMappingContext, 0);
		}
		break;
	default:
		UE_LOG(LogTemp, Error, TEXT("Dont Exsist State"));
		break;
	}
}

void ACFCraneController::SetupPlayerInput()
{
	UE_LOG(LogTemp, Warning, TEXT("ACFCraneController::SetupPlayerInput() Called."));
	if (InputComponent)
	{
		if (UEnhancedInputComponent* EnhancedInputComponent = CastChecked<UEnhancedInputComponent>(InputComponent))
		{
			if (!IsInputActionsBound)
			{
				ACFCraneBase* PlayerOwner = Cast<ACFCraneBase>(Player);
				UCFCommandComponent* CmdComp = PlayerOwner ? PlayerOwner->GetCommandComponent() : nullptr;
				EnhancedInputComponent->BindAction(InputActions->CraneMove, ETriggerEvent::Triggered, this, &ACFCraneController::CraneMove);

				EnhancedInputComponent->BindAction(InputActions->CraneCatch, ETriggerEvent::Started, this, &ACFCraneController::CraneCatch);
				//EnhancedInputComponent->BindAction(InputActions->Pause, ETriggerEvent::Started, this, &ACFCraneController::Pause);

				EnhancedInputComponent->BindAction(StanAction, ETriggerEvent::Triggered, this, &ACFCraneController::CraneRecovery);

				EnhancedInputComponent->BindAction(GrabEachOtherAction, ETriggerEvent::Triggered, this, &ACFCraneController::CraneGrabEachOtherBattle);

				if (CmdComp)
				{
					EnhancedInputComponent->BindAction(InputActions->Command, ETriggerEvent::Triggered, CmdComp, &UCFCommandComponent::InputCommand);
				}
				IsInputActionsBound = true;
			}
		}
		
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(this->GetLocalPlayer()))
		{
			UE_LOG(LogTemp, Warning, TEXT("Attempting to add CraneMappingContext."));
			Subsystem->AddMappingContext(CraneMappingContext, 0);
		}
	}
}

void ACFCraneController::OnPossess(APawn* InPawn)
{
	Super::OnPossess(InPawn);

	UE_LOG(LogTemp, Warning, TEXT("Possess"));
	if (ACFPlayer* InPlayer = Cast<ACFPlayer>(InPawn))
	{
		Player = InPlayer;
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("Possessed pawn is not ACFPlayer!"));
		return;
	}

	CGrabEachOtherIncreaseValue = Player->GrabEachOtherIncreaseValue;
	CGrabEachOtherWinValue = Player->GrabEachOtherWinValue;
	GripName = Player->CGripName;
	SetupPlayerInput();
}

void ACFCraneController::OnUnPossess()
{
	// Possessが解除されるときに呼ばれる
	UE_LOG(LogTemp, Warning, TEXT("OnUnPossess Called. Clearing input context..."));

	if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(GetLocalPlayer()))
	{
		if (CraneMappingContext)
		{
			Subsystem->RemoveMappingContext(CraneMappingContext);
		}
	}

	Super::OnUnPossess();
}

void ACFCraneController::CraneMove(const FInputActionValue& Value)
{
	if (Player == NULL)	return;
	FVector2D InputValue = Value.Get<FVector2D>();
	float Angle = FMath::Atan2(InputValue.Y, InputValue.X) * 180 / PI;

	if (abs(InputValue.X) <= DeadZone && abs(InputValue.Y) <= DeadZone)
	{
		return;
	}
	Player->Move(Value * CMoveSpeed);
}

void ACFCraneController::CraneCatch()
{
	if (Player == NULL)	return;
	UE_LOG(LogTemp, Log, TEXT("Catch Start!"));
	isCatch = true;
	Player->Catch();
}

void ACFCraneController::CraneRelease()
{
	//if (Player == NULL)	return;
	//Player->Release();
	//isCatch = false;
}

void ACFCraneController::CraneRecovery(const FInputActionValue& Value)
{
	const float InputValue = Value.Get<float>();
	const float Threshold = 0.7f;

	// 現在の入力が右方向で、前回が左方向または中央だった場合
	if (InputValue > Threshold && LastStruggleDirection <= 0)
	{
		LastStruggleDirection = 1; // 方向を「右」に更新

		UGameplayStatics::ApplyDamage(
			Player,
			-CRecoveryValue,
			this,
			this,
			UDamageType::StaticClass()
		);

		//Player->Recovery();
		UE_LOG(LogTemp, Warning, TEXT("Struggle Right! Calling AddDamage."));
	}
	// 現在の入力が左方向で、前回が右方向または中央だった場合
	else if (InputValue < -Threshold && LastStruggleDirection >= 0)
	{
		LastStruggleDirection = -1;

		//Player->AddDamage(
		//	Player,
		//	-CRecoveryValue,
		//	this,
		//	this,
		//	UDamageType::StaticClass()
		//);
		UGameplayStatics::ApplyDamage(
			Player,
			-CRecoveryValue,
			this,
			this,
			UDamageType::StaticClass()
		);

		//Player->Recovery();
		UE_LOG(LogTemp, Warning, TEXT("Struggle Left! Calling AddDamage."));
	}
	// スティックが中央に戻った場合、方向をリセット
	else if (FMath::Abs(InputValue) < Threshold)
	{
		LastStruggleDirection = 0;
	}
}

void ACFCraneController::CraneGrabEachOtherBattle(const FInputActionValue& Value)
{
	const float InputValue = Value.Get<float>();
	const float Threshold = 0.7f;

	// 現在の入力が右方向で、前回が左方向または中央だった場合
	if (InputValue > Threshold && LastStruggleDirection <= 0)
	{
		LastStruggleDirection = 1; // 方向を「右」に更新

		ClashingParameterValue = UKismetMaterialLibrary::GetScalarParameterValue(
			GetWorld(),
			ClashingParameter,
			FName("vlashing_value")
		);
		ClashingParameterValue += CGrabEachOtherIncreaseValue;

		UKismetMaterialLibrary::SetScalarParameterValue(
			GetWorld(),
			ClashingParameter,
			FName("vlashing_value"),
			ClashingParameterValue
		);

		UE_LOG(LogTemp, Warning, TEXT("Struggle Right! Calling AddDamage."));
	}
	// 現在の入力が左方向で、前回が右方向または中央だった場合
	else if (InputValue < -Threshold && LastStruggleDirection >= 0)
	{
		LastStruggleDirection = -1;
		
		ClashingParameterValue = UKismetMaterialLibrary::GetScalarParameterValue(
			GetWorld(),
			ClashingParameter,
			FName("vlashing_value")
		);
		ClashingParameterValue += CGrabEachOtherIncreaseValue;

		UKismetMaterialLibrary::SetScalarParameterValue(
			GetWorld(),
			ClashingParameter,
			FName("vlashing_value"),
			ClashingParameterValue
		);

		UE_LOG(LogTemp, Warning, TEXT("Struggle Left! Calling AddDamage."));
	}
	// スティックが中央に戻った場合、方向をリセット
	else if (FMath::Abs(InputValue) < Threshold)
	{
		LastStruggleDirection = 0;
	}

	if (ClashingParameterValue >= 100 || ClashingParameterValue <= 0)
	{
		if (CGrabEachOtherWinValue == 100)
		{
			UE_LOG(LogTemp, Log, TEXT("Win"));
			// TODO: Widget取り除き　勝ち処理
			//Player->GrabWin();
		}
		else
		{
			UE_LOG(LogTemp, Log, TEXT("Lose"));
			// TODO:　負け処理
			//Player->GrabLose();
		}
		FTimerHandle handle;
		FTimerManager& TimerManager = GetWorldTimerManager();

		FTimerDelegate TimerDelegate;

		TimerDelegate.BindUFunction(this, FName("ChangeIMC"), ECFCraneActionType::Idle);
		TimerManager.SetTimer(handle, TimerDelegate, 0.3f, false);
	}
}


void ACFCraneController::GetPrize(float Money)
{
	AddMoney(Money);
}

float ACFCraneController::GetMoney()
{
	return CMoney;
}

void ACFCraneController::SetCGripValue(float Value)
{
	CGripValue = Value;
}

void ACFCraneController::AddMoney(const float AddMoney)
{
	UE_LOG(LogTemp, Log, TEXT("PlusMonety : %f"), AddMoney);
	CMoney += AddMoney;
	UE_LOG(LogTemp, Log, TEXT("Monety : %f"), CMoney);
}

void ACFCraneController::ResetMoney()
{
	CMoney = 0;
}

void ACFCraneController::CallRelease()
{
	CraneRelease();
}

void ACFCraneController::CraneUpper()
{
}

void ACFCraneController::CraneRotateAttack()
{
}
