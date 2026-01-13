// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "CFInputDirection.h"

/**
 * 
 */
class NEWGAMEPROJECT_API CFCommandList
{
public:
	TArray<ECFInputDirection> LeftUpper =
	{
		ECFInputDirection::Left,
		ECFInputDirection::Right,
		ECFInputDirection::Left,
	};

	TArray<ECFInputDirection> RightUpper =
	{
		ECFInputDirection::Right,
		ECFInputDirection::Left,
		ECFInputDirection::Right,
	};

	TArray<ECFInputDirection> FrontUpper =
	{
		ECFInputDirection::Down,
		ECFInputDirection::Up,
		ECFInputDirection::Down,
	};

	TArray<ECFInputDirection> BackUpper =
	{
		ECFInputDirection::Up,
		ECFInputDirection::Down,
		ECFInputDirection::Up,
	};

	TArray<ECFInputDirection> LeftRotateAttack_N =
	{
		ECFInputDirection::Left,
		//ECFInputDirection::LeftUp,
		ECFInputDirection::Up,
		//ECFInputDirection::RightUp,
		ECFInputDirection::Right,
		//ECFInputDirection::RightDown,
		ECFInputDirection::Down,
		//ECFInputDirection::LeftDown,
		//ECFInputDirection::Left,
	};

	TArray<ECFInputDirection> LeftRotateAttack_R =
	{
		ECFInputDirection::Left,
		//ECFInputDirection::LeftUp,
		ECFInputDirection::Down,
		//ECFInputDirection::RightUp,
		ECFInputDirection::Right,
		//ECFInputDirection::RightDown,
		ECFInputDirection::Up,
		//ECFInputDirection::LeftDown,
		//ECFInputDirection::Left,
	};

	TArray<ECFInputDirection> RightRotateAttack_N =
	{
		ECFInputDirection::Right,
		//ECFInputDirection::RightUp,
		ECFInputDirection::Down,
		//ECFInputDirection::LeftUp,
		ECFInputDirection::Left,
		//ECFInputDirection::LeftDown,
		ECFInputDirection::Up,
		//ECFInputDirection::RightDown,
		//ECFInputDirection::Right,
	};

	TArray<ECFInputDirection> RightRotateAttack_R =
	{
		ECFInputDirection::Right,
		//ECFInputDirection::RightUp,
		ECFInputDirection::Up,
		//ECFInputDirection::LeftUp,
		ECFInputDirection::Left,
		//ECFInputDirection::LeftDown,
		ECFInputDirection::Down,
		//ECFInputDirection::RightDown,
		//ECFInputDirection::Right,
	};
};
