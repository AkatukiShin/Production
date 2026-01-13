#include "Crane/CFPlayerState.h"
#include "Crane/CFPlayer.h"
#include "Crane/CFCraneData.h"
#include "Crane/CFCraneController.h"
#include "Crane/CFCraneBase.h"
#include "GameFramework/Character.h"
#include "Kismet/KismetMaterialLibrary.h"

void ACFPlayerState::BeginPlay()
{
	Super::BeginPlay();
	if (CraneDatas)
	{
		HP = CraneDatas->Hp;
	}
}

void ACFPlayerState::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
	Super::GetLifetimeReplicatedProps(OutLifetimeProps);
}

void ACFPlayerState::ModifyHP(float Delta)
{
	UE_LOG(LogTemp, Log, TEXT("ModifyHP Start"));
	APawn* MyPawn = GetPawn();

	Player = Cast<ACFCraneBase>(MyPawn);

	if (Player == NULL)
	{
		UE_LOG(LogTemp, Error, TEXT("Player (Pawn) NULL - ModifyHP Failed"));
		return;
	}

	if (Player->ActorHasTag(FName("1P")))
	{
		HpParameterName = "HP_p1";
	}
	else
	{
		HpParameterName = "HP_p2";
	}

	UE_LOG(LogTemp, Log, TEXT("ModifyHP	1"));


	FString StateName = UEnum::GetValueAsString(Player->GetCurrentState());

	UE_LOG(LogTemp, Log, TEXT("CurrentState: %s"), *StateName);

	//if (Player->GetCurrentState() == ECFCraneActionType::Stan)	return;

	UE_LOG(LogTemp, Log, TEXT("ModifyHP 2"));

	Player->Release();

	const float OldHP = HP;
	const float MaxHP = GetMaxHP();
	HP = FMath::Clamp(HP - Delta, 0.f, MaxHP);

	UE_LOG(LogTemp, Log, TEXT("HP : %f"), HP);

	/*
	UKismetMaterialLibrary::SetScalarParameterValue(
		GetWorld(),
		HpParameter,
		HpParameterName,
		HP
	);
	*/

	AController* OwnerController = Cast<AController>(GetOwner());

	ACFCraneController* HumanController = Cast<ACFCraneController>(OwnerController);

	UE_LOG(LogTemp, Log, TEXT("ModifyHP 3"));

	if (OldHP > 0.f && HP <= 0.f)
	{
		if (Player != NULL)
		{
			Player->SetCurrentState(ECFCraneActionType::Stan);

			if (HumanController)
			{
				HumanController->ChangeIMC(ECFCraneActionType::Stan);
			}

			CraneBroken();
			UE_LOG(LogTemp, Log, TEXT("Stan!"));
		}
	}

	// --- ‰ñ•œ”»’è ---
	if (OldHP > 0.f && HP >= MaxHP)
	{
		if (HumanController)
		{
			HumanController->ChangeIMC(ECFCraneActionType::Idle);
			Player->SetCurrentState(ECFCraneActionType::Idle);
		}

		CraneCured();
		UE_LOG(LogTemp, Log, TEXT("Recovery!"));
	}
}

float ACFPlayerState::GetMaxHP() const
{
	return CraneDatas->Hp;
}

void ACFPlayerState::CraneBroken_Implementation()
{
}

void ACFPlayerState::HpReset()
{
	HP = GetMaxHP();
	UKismetMaterialLibrary::SetScalarParameterValue(
		GetWorld(),
		HpParameter,
		HpParameterName,
		HP
	);
}

void ACFPlayerState::CraneCured_Implementation()
{
}

