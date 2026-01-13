// Fill out your copyright notice in the Description page of Project Settings.


#include "Crane/AI/CFEnemy.h"

void ACFEnemy::XYMove(const FVector2D& Value)
{
	MoveComponent->XYMove(Value);
}
