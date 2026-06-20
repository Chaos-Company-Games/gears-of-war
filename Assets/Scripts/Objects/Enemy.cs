using UnityEngine;

//Class for managing combat opponents in game.
public class Enemy 
{
    int maxHP { get; set; }
    int HP { get; set; }




    #region HealthFunctions
    //Reduce HP by amount passed in
    public void takeDamage(int dmg)
    {
        HP = HP - dmg;
        if (HP < 0)
        {
            HP = 0;
            //death trigger
        }
    }

    public void getHealed(int rmdy)
    {
        HP = HP + rmdy;
        if( HP > maxHP)
        {
            HP = maxHP;
        }
    }
    #endregion
}
