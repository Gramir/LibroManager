## Plan: Implement Screenplay Pattern for E2E Tests

This plan restructures the E2E project to support both POM and Screenplay architectures. The existing POM tests will be moved to a `POM/` subfolder, and a new `Screenplay/` implementation will be created using native C# with Playwright (no external libraries). Both architectures will share the common PlaywrightServerFixture infrastructure.

**Phases 9**

1. **Phase 1: Restructure Project - Move POM Content**
    - **Objective:** Move all existing POM code to a `POM/` subfolder while maintaining functionality
    - **Files/Functions to Modify/Create:** 
        - Create `POM/Pages/` folder and move LoginPage.cs, MainPage.cs, UserManagementPage.cs
        - Create `POM/Tests/` folder and move LoginPage/, MainPage/, AdminPage/ test folders
        - Update namespace references from `LibroManager.Tests.E2E.Pages` to `LibroManager.Tests.E2E.POM.Pages`
        - Update namespace references in test files
    - **Tests to Write:** None (existing tests must pass after refactoring)
    - **Steps:**
        1. Create POM/Pages/ and POM/Tests/ directory structure
        2. Move Pages/*.cs files to POM/Pages/
        3. Move test folders (LoginPage/, MainPage/, AdminPage/) to POM/Tests/
        4. Update all namespace declarations and using statements
        5. Run existing tests to verify they still pass

2. **Phase 2: Create Screenplay Core - Interfaces and Actor**
    - **Objective:** Implement the foundational Screenplay pattern interfaces and Actor class
    - **Files/Functions to Modify/Create:**
        - `Screenplay/Core/Abilities/IAbility.cs` - Marker interface for abilities
        - `Screenplay/Core/Interactions/ITask.cs` - Interface with `PerformAsAsync(Actor)` method
        - `Screenplay/Core/Interactions/IQuestion.cs` - Generic interface with `AnsweredByAsync(Actor)` method
        - `Screenplay/Core/Actors/Actor.cs` - Main actor class with `WhoCan()`, `AttemptsToAsync()`, `AsksForAsync()` methods
    - **Tests to Write:**
        - `ActorTests.cs` - Test actor can hold abilities and perform basic tasks
    - **Steps:**
        1. Write unit test for Actor holding abilities
        2. Create IAbility marker interface
        3. Create ITask interface with PerformAsAsync
        4. Create IQuestion<T> interface with AnsweredByAsync
        5. Create Actor class with ability management
        6. Run test to verify Actor works correctly

3. **Phase 3: Implement BrowseTheWeb Ability**
    - **Objective:** Create the Playwright-based ability that enables browser automation
    - **Files/Functions to Modify/Create:**
        - `Screenplay/Core/Abilities/BrowseTheWeb.cs` - Holds IPage and BaseUrl, provides `Using()` and `As()` static methods
    - **Tests to Write:**
        - `BrowseTheWebTests.cs` - Test ability can be created and retrieved from actor
    - **Steps:**
        1. Write unit test for BrowseTheWeb ability creation and retrieval
        2. Create BrowseTheWeb class implementing IAbility
        3. Implement Using() factory method
        4. Implement As() helper method for extracting from Actor
        5. Run test to verify ability works correctly

4. **Phase 4: Implement Atomic Tasks**
    - **Objective:** Create low-level, reusable task classes for common browser interactions
    - **Files/Functions to Modify/Create:**
        - `Screenplay/Core/Interactions/Tasks/Navigate.cs` - Navigate to URL/path
        - `Screenplay/Core/Interactions/Tasks/Click.cs` - Click on locator
        - `Screenplay/Core/Interactions/Tasks/Fill.cs` - Fill input with value (fluent API)
        - `Screenplay/Core/Interactions/Tasks/WaitForUrl.cs` - Wait for URL pattern
    - **Tests to Write:**
        - `NavigateTaskTests.cs` - Integration test with real browser
        - `ClickTaskTests.cs` - Integration test with real browser
        - `FillTaskTests.cs` - Integration test with real browser
    - **Steps:**
        1. Write integration test for Navigate task
        2. Implement Navigate task with ToPath(), ToLoginPage() static methods
        3. Write integration test for Click task
        4. Implement Click task with On() static method
        5. Write integration test for Fill task
        6. Implement Fill task with fluent The().With() API
        7. Implement WaitForUrl task
        8. Run all task tests to verify functionality

5. **Phase 5: Implement Questions**
    - **Objective:** Create question classes for querying page state
    - **Files/Functions to Modify/Create:**
        - `Screenplay/Core/Questions/TheText.cs` - Get inner text of element
        - `Screenplay/Core/Questions/TheVisibility.cs` - Check if element is visible
        - `Screenplay/Core/Questions/TheAttribute.cs` - Get element attribute value
    - **Tests to Write:**
        - `TheTextTests.cs` - Integration test for text retrieval
        - `TheVisibilityTests.cs` - Integration test for visibility check
        - `TheAttributeTests.cs` - Integration test for attribute retrieval
    - **Steps:**
        1. Write integration test for TheText question
        2. Implement TheText question with Of() static method
        3. Write integration test for TheVisibility question
        4. Implement TheVisibility question with Of() static method
        5. Write integration test for TheAttribute question
        6. Implement TheAttribute question with Of().Named() fluent API
        7. Run all question tests to verify functionality

6. **Phase 6: Create UI Models**
    - **Objective:** Create static classes with locator definitions for each page
    - **Files/Functions to Modify/Create:**
        - `Screenplay/UI/LoginUI.cs` - Locators for login page (EmailInput, PasswordInput, LoginButton, ErrorMessage)
        - `Screenplay/UI/MainPageUI.cs` - Locators for main page (NavbarTitle, MainHeader, LoginLink, UserLogged)
        - `Screenplay/UI/UserManagementUI.cs` - Locators for user management page (AddUserButton, inputs, SaveButton, etc.)
    - **Tests to Write:** None (locators tested via integration tests)
    - **Steps:**
        1. Create LoginUI with static methods returning ILocator
        2. Create MainPageUI with static methods returning ILocator
        3. Create UserManagementUI with static methods returning ILocator
        4. Verify locators match existing POM page implementations

7. **Phase 7: Create High-Level Tasks**
    - **Objective:** Compose atomic tasks into domain-specific actions
    - **Files/Functions to Modify/Create:**
        - `Screenplay/Tasks/LoginAsUser.cs` - Composite task with WithCredentials(), AsAdmin() methods
        - `Screenplay/Tasks/CreateUser.cs` - Composite task for user creation workflow
    - **Tests to Write:**
        - `LoginAsUserTests.cs` - Integration test for complete login flow
        - `CreateUserTests.cs` - Integration test for user creation flow
    - **Steps:**
        1. Write integration test for LoginAsUser task
        2. Implement LoginAsUser composing Navigate, Fill, Click tasks
        3. Write integration test for CreateUser task
        4. Implement CreateUser composing navigation and form interactions
        5. Run all high-level task tests

8. **Phase 8: Create ScreenplayTestBase and First Test**
    - **Objective:** Create test infrastructure and implement first Screenplay test
    - **Files/Functions to Modify/Create:**
        - `Screenplay/Tests/ScreenplayTestBase.cs` - Base class with CreateActorAsync(), RunScreenplayTestAsync() methods
        - `Screenplay/Tests/LoginScreenplayTest.cs` - Replicate Admin_Should_Login_Successfully test
    - **Tests to Write:**
        - `Admin_Should_Login_Successfully` - Screenplay version of login test
    - **Steps:**
        1. Create ScreenplayTestBase extending E2ETestBase
        2. Implement CreateActorAsync() method
        3. Implement RunScreenplayTestAsync() wrapper method
        4. Create LoginScreenplayTest class
        5. Implement Admin_Should_Login_Successfully using Screenplay pattern
        6. Run test to verify it passes

9. **Phase 9: Complete Remaining Screenplay Tests**
    - **Objective:** Replicate all POM tests using Screenplay pattern
    - **Files/Functions to Modify/Create:**
        - `Screenplay/Tests/MainPageScreenplayTest.cs` - Replicate MainPage_Should_Display_Title_And_LoginLink
        - `Screenplay/Tests/AdminUserManagementScreenplayTest.cs` - Replicate Admin_Can_Create_Bibliotecario_And_Login
    - **Tests to Write:**
        - `MainPage_Should_Display_Title_And_LoginLink` - Screenplay version
        - `Admin_Can_Create_Bibliotecario_And_Login` - Screenplay version
    - **Steps:**
        1. Create MainPageScreenplayTest class
        2. Implement MainPage_Should_Display_Title_And_LoginLink test
        3. Run MainPage test to verify it passes
        4. Create AdminUserManagementScreenplayTest class
        5. Implement Admin_Can_Create_Bibliotecario_And_Login test
        6. Run AdminUserManagement test to verify it passes
        7. Run all E2E tests (both POM and Screenplay) to verify full suite passes

**Open Questions** - ANSWERED ✅
1. ~~¿Deseas agregar logging/reporting específico para el patrón Screenplay?~~ **Respuesta: B - Crear logger específico para acciones del Actor**
2. ~~¿Prefieres que las Tasks arrojen excepciones descriptivas personalizadas?~~ **Respuesta: Personalizadas**
3. ~~¿Los tests de visual regression también deben replicarse en Screenplay?~~ **Respuesta: Sí, 1:1 para comparar ambos patrones**
4. **Comentarios**: Agregar comentarios explicativos en INGLÉS en todo el código
