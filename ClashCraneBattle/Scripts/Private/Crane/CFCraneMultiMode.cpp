// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#include "Crane/CFCraneMultiMode.h"
#include "EnhancedInputSubsystems.h"
#include "Crane/CFCraneController.h"
#include "GameFramework/PlayerController.h"

ACFCraneMultiMode::ACFCraneMultiMode()
{
	DefaultPawnClass = NULL;
	PlayerControllerClass = ACFCraneController::StaticClass();
}

