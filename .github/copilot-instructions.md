# GitHub Copilot Instructions for AnimalHarvestingSpot (Continued)

## Mod Overview and Purpose

AnimalHarvestingSpot (Continued) is a mod for RimWorld that enhances animal management by providing specific spots designated for various animal-related tasks. Originally developed by -210, this version has been updated to improve functionality and user experience. The mod seamlessly integrates into the gameplay by allowing pawns to select the nearest appropriate spot for the desired animal-related action.

## Key Features and Systems

- **New Spots:** Adds four new spots for specific tasks: Milking, Shearing, Slaughtering, and Training. Each spot is designated for a particular action, optimizing pawns' efficiency.
- **Icon Integration:** Utilizes icons created by Those Icons and Freepik for visual clarity.
- **Location Efficiency:** Pawns choose the closest valid spot for their intended action, enhancing gameplay flow.
- **Zone Integration:** Spots are now placeable in zones, providing more flexibility in colony design.
- **Localization Support:** Includes a German translation, thanks to contributor Dimos.
- **All-in-One Spot:** The original harvesting spot can be used for all actions except training.

## Coding Patterns and Conventions

- **Inheritance Structure:** The mod's classes such as `JobDriver_Milk_OnSpot` and `JobDriver_Shear_OnSpot` inherit from base classes like `AnimalHarvestingSpot` to ensure shared functionality and reduce redundant code.
- **Naming Conventions:** Follow C# naming conventions for classes (PascalCase) and methods (camelCase).
- **Modular Design:** Separate classes and files for different functionalities to maintain clarity and ease of maintenance.

## XML Integration

- Define animal harvesting spot configurations through XML to allow for easy adjustments and additions without recompiling code.
- Use XML for localizing strings, enabling straightforward translation processes and multi-language support.

## Harmony Patching

- Utilize Harmony to safely apply patches ensuring compatibility with other mods and the base game.
- Apply patches to modify existing game mechanics non-intrusively, allowing for extension or replacement of behaviors without conflicts.

## Suggestions for Copilot

- **Pattern Recognition:** Use Copilot to detect and suggest consistent patterns in similar methods and classes, such as gathering resources or assigning jobs.
- **Code Completion:** Leverage Copilot to autofill code based on detected patterns in method structures and repeated logic.
- **Error Prediction:** Implement AI suggestions to predict and address potential errors in integration, such as XML tag mismatches or unpatched methods.
- **Localization Automation:** Assisting in identifying all strings requiring translation for seamless addition to the localization XML files.
- **Harmony Usage:** Offer suggestions for commonly used Harmony patching patterns such as prefix, postfix, and transpiler methods.

Ensure that you continue to adapt and leverage Copilot’s capabilities to maintain a high-efficiency workflow, while also contributing back improvements and fixes to this repository. For any issues or contributions, use the available channels, such as GitHub issues or Discord, to ensure swift and coordinated development efforts.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
