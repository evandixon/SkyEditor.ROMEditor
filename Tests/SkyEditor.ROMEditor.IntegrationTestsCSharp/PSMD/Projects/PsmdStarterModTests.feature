Feature: PsmdStarterModTests

Scenario: Verify the successful build of the PSMD Starter Mod Project
Given I have a DS Mod solution
And I initialize the solution with a PSMD-US.3ds ROM
And The solution has a PsmdStarterMod project
And The modpack project will output a decrypted ROM only
When I build the project
And I unpack the resulting ROM
Then The personality test script should have been properly patched
