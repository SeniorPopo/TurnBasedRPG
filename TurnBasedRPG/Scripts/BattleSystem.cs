using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, NECROMANCERTURN, THIEFTURN, ENEMYTURN, MINIONTURN, WON, LOST, RAN }

public class BattleSystem : MonoBehaviour
{

    public GameObject necromancerPrefab;
    public GameObject thiefPrefab;
    public GameObject enemyPrefab;
    public GameObject minionPrefab;

    public Transform necromancerSide;
    public Transform thiefSide;
    public Transform enemySide;
    public Transform meleeDistance;
    public Transform minionSpawn;

    public TextMeshProUGUI dialogueText;

    public BattleState state;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public CameraShake cameraShake;
    public FlashBang flashBang;

    private Animator necromancerAnimationController;
    private Animator thiefAnimationController;
    private Animator skeletonAnimationController;
    private Animator batAnimationController;

    public AudioSource audioSource;
    public AudioClip[] audioClipArray;

    Unit enemyUnit;
    Unit minionUnit;
    Thief playerThief;
    Necromancer playerNecromancer;

    bool isPlayerTurn = true;

    //Animation States
    const string NECROMANCER_STAFF_IDLE = "NecromancerStaffIdle";
    const string NECROMANCER_CAST = "NecromancerCast";
    const string NECROMANCER_DIE = "NecromancerDie";
    const string RED_IDLE = "RedBattleIdle";
    const string RED_MELEE = "RedDashAttack";
    const string RED_BOW = "RedBowAttack";
    const string RED_THROW = "RedThrow";
    const string RED_ANGRY = "RedAngry";
    const string RED_DEATH = "RedDeath";
    const string SKELETON_IDLE = "SkeletonIdle";
    const string SKELETON_ATTACK = "SkeletonAttack";
    string ENEMY_ATTACK;
    string ENEMY_IDLE;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());

    }
    /**
     * SetupBattle puts both the player and enemy unit on the correct sides of the scene, updates the prefab stats, displays the enemy's name, then starts the player's turn
     */
    IEnumerator SetupBattle()
    {
        minionPrefab.SetActive(false);
        
        GameObject playerGO = Instantiate(necromancerPrefab, necromancerSide);
        playerNecromancer = playerGO.GetComponent<Necromancer>(); //spawns player
        playerNecromancer.UpdatePrefab(); //updates prefab's stats
        playerHUD.SetHUD(playerNecromancer); //updates the HUD
        necromancerAnimationController = playerNecromancer.GetComponentInChildren<Animator>();
        necromancerAnimationController.Play(NECROMANCER_STAFF_IDLE);

        GameObject thiefGO = Instantiate(thiefPrefab, thiefSide);
        playerThief = thiefGO.GetComponent<Thief>(); //spawns player
        playerThief.UpdatePrefab(); //updates prefab's stats
        thiefAnimationController = playerThief.GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        thiefAnimationController.Play(RED_IDLE);
        GameObject enemyGO = Instantiate(enemyPrefab, enemySide); 
        enemyUnit = enemyGO.GetComponent<Unit>(); //spawns enemy
        batAnimationController = enemyUnit.GetComponentInChildren<Animator>();
        ENEMY_IDLE = enemyUnit.GetUnitName() + "_Idle";
        batAnimationController.Play(ENEMY_IDLE);
        enemyUnit.UpdatePrefab(); //updates prefab's stats
        dialogueText.text = enemyUnit.GetUnitName();
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);
      
        state = BattleState.NECROMANCERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        // Damage the enemy
        if (state == BattleState.NECROMANCERTURN)
        {
            enemyUnit.TakeDamage(playerNecromancer.DealDamage());
            playerNecromancer.Heal(1); //Necromancer has built in life steal
            //updates HUDs to reflect damage
            playerHUD.SetHP(playerNecromancer.GetCurrentHP());
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
            state = BattleState.THIEFTURN;
            Debug.Log(state);
        }
        else if (state == BattleState.THIEFTURN)
        {
            int critRNG = Random.Range(1, 10); //Thief can crit
            Debug.Log("Thief coroutine");
            if (critRNG <= playerThief.Luck)
            {
                dialogueText.SetText("A Critical Hit!");
                thiefAnimationController.Play(RED_THROW);
                yield return new WaitForSeconds(1f);
                audioSource.PlayOneShot(audioClipArray[0] , 1f);
                StartCoroutine(cameraShake.Shake(1f, 0.75f));
                flashBang.ScreenFlash(.25f, .75f, Color.white);
                enemyUnit.TakeDamage(playerThief.DealDamage()*2);
                yield return new WaitForSeconds(2f);
            }
            else
            {
                thiefAnimationController.Play(RED_MELEE);
                yield return new WaitForSeconds(.5f);
                audioSource.PlayOneShot(audioClipArray[2] , 1f);
                flashBang.ScreenFlash(.1f, .5f, Color.white);
                enemyUnit.TakeDamage(playerThief.DealDamage());
            }
            playerHUD.SetHP(playerThief.GetCurrentHP());
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
            state = BattleState.ENEMYTURN;

        }
        dialogueText.text = "The attack is successful!";


        // Check if the enemy is dead
        if (enemyUnit.GetCurrentHP() <= 0)
        {
            //End the Battle
            state = BattleState.WON;
            EndBattle();
        }
        else if (minionUnit)
        {
            // if a minion is spawned
            state = BattleState.MINIONTURN;
            StartCoroutine(MinionTurn());
        }
        else if (state == BattleState.THIEFTURN)
        {
            PlayerTurn();
            yield break;
        }
        else
        {
            //Enemy turn
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerSpellOne()
    {
        if (state == BattleState.NECROMANCERTURN) //Necromancer sacrifices health to deal double damage
        {
            necromancerAnimationController.Play(NECROMANCER_CAST);
            necromancerAnimationController.Play(NECROMANCER_STAFF_IDLE);
            enemyUnit.TakeDamage(playerNecromancer.DealDamage()*2);
            playerNecromancer.TakeDamage(5);
            playerNecromancer.SpendMana(5);
            playerHUD.SetHP(playerNecromancer.GetCurrentHP());
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
            playerHUD.SetMana(playerNecromancer.GetCurrentMana());
            dialogueText.text = "The spell is successful!";
            state = BattleState.THIEFTURN;
        }

        else if (state == BattleState.THIEFTURN) //Thief doubles crit rate
        {
            thiefAnimationController.Play(RED_ANGRY);
            playerThief.CriticalEye();
            playerThief.SpendMana(5);
            playerHUD.SetMana(playerThief.GetCurrentMana());
            dialogueText.SetText("Critical Rate Doubled!");
            Debug.Log(playerThief.Luck);
            state = BattleState.ENEMYTURN;
        }



        yield return new WaitForSeconds(2f);

        // Check if the enemy is dead
        if (enemyUnit.GetCurrentHP() <= 0)
        {
            //End the Battle
            state = BattleState.WON;
            EndBattle();
        }
        // If a minion is on the field, its turn starts
        else if(minionUnit){
            state = BattleState.MINIONTURN;
            StartCoroutine(MinionTurn());
        }
        else if (state == BattleState.THIEFTURN)
        {
            PlayerTurn();
        }
        else
        {
            //Enemy turn
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerSpellTwo()
    {
        if (state == BattleState.NECROMANCERTURN && !playerNecromancer.GetMinionSummoned()) //Necromancer summons a skeleton minion
        {
            necromancerAnimationController.Play(NECROMANCER_CAST);
            necromancerAnimationController.Play(NECROMANCER_STAFF_IDLE);
            playerNecromancer.SummonSkeleton();
            playerNecromancer.SpendMana(10);
            playerHUD.SetMana(playerNecromancer.GetCurrentMana());
            minionPrefab.SetActive(true);
            GameObject minionGO = Instantiate(minionPrefab, minionSpawn);
            minionUnit = minionGO.GetComponent<Unit>();
            minionUnit.UpdatePrefab();
            skeletonAnimationController = minionUnit.GetComponentInChildren<Animator>();
            skeletonAnimationController.Play(SKELETON_IDLE);
            dialogueText.SetText("A minion is summoned!");
            yield return new WaitForSeconds(2f);
            StartCoroutine(MinionTurn());
        }

        else if (state == BattleState.THIEFTURN) //Thief analyzes the enemy, revealing its starts
        {
            dialogueText.SetText(enemyUnit.GetUnitName());
            yield return new WaitForSeconds(2f);
            dialogueText.SetText("HP: " + enemyUnit.GetCurrentHP() + " / " + enemyUnit.GetMaxHP());
            yield return new WaitForSeconds(2f);
            dialogueText.SetText("MP: " + enemyUnit.GetCurrentMana() + " / " + enemyUnit.GetMaxMana());
            yield return new WaitForSeconds(2f);
            playerThief.SpendMana(10);
            playerHUD.SetMana(playerThief.GetCurrentMana());
            StartCoroutine(EnemyTurn());
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        // Check if the enemy is dead
        if (enemyUnit.GetCurrentHP() <= 0)
        {
            //End the Battle
            state = BattleState.WON;
            EndBattle();
        }
        // If a minion is on the field, its turn starts
        else if (minionUnit)
        {
            state = BattleState.MINIONTURN;
            StartCoroutine(MinionTurn());
        }
        else if (state == BattleState.THIEFTURN)
        {
            PlayerTurn();
        }
        else
        {
            //Enemy turn
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerSpellThree()
    {
        if (state == BattleState.NECROMANCERTURN) //Necromancer bleeds the target
        {
            necromancerAnimationController.Play(NECROMANCER_CAST);
            necromancerAnimationController.Play(NECROMANCER_STAFF_IDLE);
            enemyUnit.SetIsBleeding(true);
            playerNecromancer.SpendMana(5);
            playerHUD.SetMana(playerNecromancer.GetCurrentMana());
            enemyUnit.GetIsBleeding();
            state = BattleState.THIEFTURN;

        }
        else if (state == BattleState.THIEFTURN) //Thief deals half damage but stuns the target for a turn
        {
            thiefAnimationController.Play(RED_BOW);
            yield return new WaitForSeconds(.5f);
            audioSource.PlayOneShot(audioClipArray[1] , 1f);
            flashBang.ScreenFlash(.1f, .5f, Color.white);
            enemyUnit.TakeDamage(playerThief.DealDamage() / 2);
            dialogueText.SetText("The enemy is stunned!");
            playerThief.SpendMana(5);
            playerHUD.SetMana(playerThief.GetCurrentMana());
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
            yield return new WaitForSeconds(2f);
            PlayerTurn();
            yield break;
        }

        dialogueText.SetText("The spell is successful!");


        yield return new WaitForSeconds(2f);

        // Check if the enemy is dead
        if (enemyUnit.GetCurrentHP() <= 0)
        {
            //End the Battle
            state = BattleState.WON;
            EndBattle();
        }
        // If a minion is on the field, its turn starts
        else if (minionUnit)
        {
            state = BattleState.MINIONTURN;
            StartCoroutine(MinionTurn());
        }
        else if (state == BattleState.THIEFTURN)
        {
            PlayerTurn();
        }
        else
        {
            //Enemy turn
            StartCoroutine(EnemyTurn());
        }

    }
    IEnumerator PlayerSpellFour()
    {
        if (state == BattleState.NECROMANCERTURN) //Necromancer blinds the enemy
        {
            necromancerAnimationController.Play(NECROMANCER_CAST);
            necromancerAnimationController.Play(NECROMANCER_STAFF_IDLE);
            enemyUnit.SetIsBlinded(true);
            playerNecromancer.SpendMana(5);
            playerHUD.SetMana(playerNecromancer.GetCurrentMana());
            enemyUnit.GetIsBleeding();
            state = BattleState.THIEFTURN;

        }
        else if (state == BattleState.THIEFTURN)
        {
            thiefAnimationController.Play(RED_BOW);
            yield return new WaitForSeconds(.5f);
            audioSource.PlayOneShot(audioClipArray[1], 1f);
            flashBang.ScreenFlash(.1f, .5f, Color.white);
            enemyUnit.TakeDamage(playerThief.DealDamage() / 2);
            dialogueText.SetText("The enemy is stunned!");
            playerThief.SpendMana(5);
            playerHUD.SetMana(playerThief.GetCurrentMana());
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
            state = BattleState.NECROMANCERTURN;
            yield return new WaitForSeconds(2f);
            PlayerTurn();
            yield break;
        }

        dialogueText.SetText("The spell is successful!");


        yield return new WaitForSeconds(2f);

        // Check if the enemy is dead
        if (enemyUnit.GetCurrentHP() <= 0)
        {
            //End the Battle
            state = BattleState.WON;
            EndBattle();
        }
        // If a minion is on the field, its turn starts
        else if (minionUnit)
        {
            state = BattleState.MINIONTURN;
            StartCoroutine(MinionTurn());
        }
        else if (state == BattleState.THIEFTURN)
        {
            PlayerTurn();
        }
        else
        {
            //Enemy turn
            StartCoroutine(EnemyTurn());
        }

    }
    IEnumerator PlayerRun()
    {
        int runRNG = Random.Range(1, 10);


        if (runRNG >= 5)
        {
            state = BattleState.RAN;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            dialogueText.text = "You couldn't get away";
            yield return new WaitForSeconds(2f);
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator MinionTurn()
    {
        dialogueText.text = "Your minion Attacks!";
        skeletonAnimationController.Play(SKELETON_ATTACK);
        yield return new WaitForSeconds(1f);
        enemyUnit.TakeDamage(minionUnit.DealDamage());
        enemyHUD.SetHP(enemyUnit.GetCurrentHP());
        yield return new WaitForSeconds(1f);
        if (enemyUnit.GetCurrentHP() <= 0)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.THIEFTURN;
            PlayerTurn();
        }
    }

    IEnumerator EnemyTurn()
    {
        // Is there a way to consolidate the enemy turn function?
        if (enemyUnit.GetIsBleeding())
        {
            dialogueText.text = enemyUnit.GetUnitName() + " takes bleeding damage";
            enemyUnit.TakeDamage(playerNecromancer.DealDamage());
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
            yield return new WaitForSeconds(2f);
        }
        if (enemyUnit.GetCurrentHP() <= 0)
        {
            state = BattleState.WON;
            EndBattle();
            yield break;
        }

        if (enemyUnit.GetIsBlinded())
        {
            // Find a way to get rid of this nested conditional
            int enemyAccuracy = Random.Range(0, 2);
            if (enemyAccuracy == 0) 
            {
                dialogueText.text = enemyUnit.GetUnitName() + " misses!";
                yield return new WaitForSeconds(2f);
                state = BattleState.NECROMANCERTURN;
                playerHUD.SetHUD(playerNecromancer);
                PlayerTurn();
                yield break;
            }
        }

        int enemyRNG = Random.Range(0, 10);

        if (enemyRNG <= 2)
        {
            dialogueText.text = enemyUnit.GetUnitName() + enemyUnit.AttackNameOne();
            ENEMY_ATTACK = enemyUnit.GetUnitName() + "_Scream";
            batAnimationController.Play(ENEMY_ATTACK);
            yield return new WaitForSeconds(1f);

            playerNecromancer.TakeDamage(enemyUnit.DealDamage());
            playerHUD.SetHP(playerNecromancer.GetCurrentHP());
            yield return new WaitForSeconds(1f);
        }

        else if (enemyRNG <= 5)
        {
            dialogueText.text = enemyUnit.GetUnitName() + enemyUnit.AttackNameOne();
            yield return new WaitForSeconds(1f);

            playerThief.TakeDamage(enemyUnit.DealDamage());
            playerHUD.SetHP(playerThief.GetCurrentHP());
            yield return new WaitForSeconds(1f);
        }
        else if (enemyRNG > 5 && enemyRNG <= 7) 
        {
            dialogueText.text = enemyUnit.GetUnitName() + enemyUnit.AttackNameTwo();
            enemyUnit.Buff();
            yield return new WaitForSeconds(1f);
        }
        else if (enemyRNG > 7 && enemyRNG <= 9)
        {
            dialogueText.text = enemyUnit.GetUnitName() + enemyUnit.AttackNameThree();
            enemyUnit.Debuff(playerNecromancer);
            enemyUnit.Debuff(playerThief);

            yield return new WaitForSeconds(1f);
        }
        else
        {
            dialogueText.text = enemyUnit.GetUnitName() + enemyUnit.AttackNameThree();
            enemyUnit.Heal(5);
            enemyHUD.SetHP(enemyUnit.GetCurrentHP());
            yield return new WaitForSeconds(1f);
        }

        if (playerThief.GetCurrentHP() <= 0 && playerNecromancer.GetCurrentHP() <= 0)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            playerHUD.SetHUD(playerNecromancer);
            playerHUD.SetHUD(playerThief);
            state = BattleState.NECROMANCERTURN;
            PlayerTurn();
            yield break;
        }
        yield break;



    }
    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            necromancerAnimationController.Play(NECROMANCER_DIE);
            thiefAnimationController.Play(RED_DEATH);
            audioSource.PlayOneShot(audioClipArray[3] , 1f);
            dialogueText.text = "You were defeated.";
        }
        else if (state == BattleState.RAN)
        {
            dialogueText.text = "Got away sucessfully";
        }
    }
    void PlayerTurn()
    {
        if(playerNecromancer.GetCurrentHP() <= 0)
        {
            state = BattleState.THIEFTURN;
        }
        if(state == BattleState.NECROMANCERTURN)
        {
            playerHUD.SetHUD(playerNecromancer);
            dialogueText.text = "Choose an action:";
            isPlayerTurn = true;
        }
        else if(state == BattleState.THIEFTURN && playerThief.GetCurrentHP() > 0)
        {
            playerHUD.SetHUD(playerThief);
            dialogueText.text = "Choose an action:";
            isPlayerTurn = true;
        }
        else
        {
            StartCoroutine(EnemyTurn());
            
        }
    }

    IEnumerator PlayerHeal()
    {
        if(state == BattleState.NECROMANCERTURN)
        {
            playerNecromancer.Heal(5);

            playerHUD.SetHP(playerNecromancer.GetCurrentHP());
            dialogueText.text = "You heal for 5 points of HP";
            state = BattleState.THIEFTURN;
            yield return new WaitForSeconds(2f);
        }
        else if(state == BattleState.THIEFTURN)
        {
            playerThief.Heal(5);

            playerHUD.SetHP(playerThief.GetCurrentHP());
            dialogueText.text = "You heal for 5 points of HP";
            state = BattleState.ENEMYTURN;
            yield return new WaitForSeconds(2f);
        }
        if (minionUnit)
        {
            state = BattleState.MINIONTURN;
            StartCoroutine(MinionTurn());
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }

    }

    public void OnAttackButton()
    {
        if (!isPlayerTurn)
            return;
        isPlayerTurn = false;
        StartCoroutine(PlayerAttack());
    }
    public void OnHealButton()
    {
        if (!isPlayerTurn)
            return;

        StartCoroutine(PlayerHeal());
    }
    public void OnSpellButton()
    {
        if (!isPlayerTurn)
            return;

        else if(playerHUD.hpSlider.value <= 5 || playerHUD.manaSlider.value < 5)
        {
            dialogueText.text = "Not enough HP or Mana";
            return;
        }
        isPlayerTurn = false;
        StartCoroutine(PlayerSpellOne());
    }

    public void OnSpellButton2()
    {
        if (!isPlayerTurn)
            return;
        else if (playerHUD.manaSlider.value < 10)
        {
            dialogueText.text = "Not enough Mana";
            return;
        }
        isPlayerTurn = false;
        StartCoroutine(PlayerSpellTwo());
    }

    public void OnSpellButton3()
    {
        if (!isPlayerTurn)
            return;
        else if(playerHUD.manaSlider.value < 5)
        {
            dialogueText.text = "Not enough Mana";
            return;
        }
        isPlayerTurn = false;
        StartCoroutine(PlayerSpellThree());
    }
    public void OnSpellButton4()
    {
        if (!isPlayerTurn)
            return;
        else if (playerHUD.manaSlider.value < 5)
        {
            dialogueText.text = "Not enough Mana";
            return;
        }
        isPlayerTurn = false;
        StartCoroutine(PlayerSpellFour());
    }

    public void OnRunButton()
    {
        if (!isPlayerTurn)
            return;
        isPlayerTurn = false;
        StartCoroutine(PlayerRun());
    }
}
