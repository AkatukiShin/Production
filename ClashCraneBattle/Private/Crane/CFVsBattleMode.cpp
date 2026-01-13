// Fill out your copyright notice in the Description page of Project Settings.


#include "Crane/CFVsBattleMode.h"
#include "Crane/CFCraneController.h"
#include "Crane/CFPlayer.h"
#include "Kismet/GameplayStatics.h"

ACFVsBattleMode::ACFVsBattleMode()
{
    
}

void ACFVsBattleMode::BeginPlay()
{
    Super::BeginPlay();

    for (int32 ControllerId = 1; ControllerId < localPlayers; ++ControllerId)
    {
        UGameplayStatics::CreatePlayer(GetWorld(), ControllerId, true);
    }
}
