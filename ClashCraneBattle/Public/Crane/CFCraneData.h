// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "CFCraneData.generated.h"

/**
 * 
 */
UCLASS()
class NEWGAMEPROJECT_API UCFCraneData : public UPrimaryDataAsset
{
	GENERATED_BODY()
	
public:
	UPROPERTY(EditAnywhere, Blueprintable, Category = "Hp")
	float Hp = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Attack|Grace")
	float CommandGraceTime = 0.18f;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Attack|Command")
	float DeadZone = 0.4f;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Attack|Upper")
	float UpperAttackValue = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Attack|Upper")
	float UpperSuccesMoney = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Attack|Rotation")
	float RotationAttackValue = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Attack|Rotation")
	float RotationSuccesValue = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Attack|Rotation")
	float GrabAttackValue = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Catch")
	float StopTime = 3.0f;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Move")
	float MoveSpeed = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Move")
	float CatchedMoveSpeed = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Move")
	float UpSpeed = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Move")
	float DownSpeed = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Kakin|Money")
	float Money = 0;

	UPROPERTY(EditAnywhere, Blueprintable, Category = "Recovery")
	float RecoveryValue = 0;
};
