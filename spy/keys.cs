bind make  alt-Left action MoveLeft '$DirectionalVelocity'
bind break alt-Left action MoveLeft 0'
bind make  alt-Right action MoveRight '$DirectionalVelocity'
bind break alt-Right action MoveRight 0'
bind make  Down action MoveBackward '$DirectionalVelocity'
bind break Down action MoveBackward 0
bind make  Up action MoveForward '$DirectionalVelocity'
bind break Up action MoveForward 0'
bind make  alt-Up action MoveUp '$DirectionalVelocity'
bind break alt-Up action MoveUp 0'
bind make  alt-Down action MoveDown '$DirectionalVelocity'
bind break alt-Down action MoveDown 0'

bind make  Left  action MoveYaw '$PositiveRotation'
bind break Left  action MoveYaw 0'
bind make  Right action MoveYaw '$NegativeRotation'
bind break Right action MoveYaw 0'
bind make  ctrl-Up    action MovePitch '$PositiveRotation'
bind break ctrl-Up    action MovePitch 0'
bind make  ctrl-Down  action MovePitch '$NegativeRotation'
bind break ctrl-Down  action MovePitch 0'
bind make  r  action MoveRoll '$PositiveRotation'
bind break r  action MoveRoll 0'
bind make  R  action MoveRoll '$NegativeRotation'
bind break R  action MoveRoll 0'
