using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

// force à mettre ces composants

public class PlayerCharacter2D : MonoBehaviour
{

    [Tooltip("la 'difficulté' pour faire avancer le perso (sa vitesse d'accélération)")]
    public float movementAcceleration = 20f;

    [Tooltip("vitesse max/s sur X")]
    public float maxMovementSpeed = 4f;

    [Tooltip("la 'difficulté' pour faire sauter le perso (force appliquée)")]
    public float jumpForce = 16f;

    [Tooltip("la force appaliquée pour la descente")]
    public float fallForce = 13f;

    [Tooltip("le layer sur lequel ça agit pour éviter que le chara capte son propre collider ")]
    public LayerMask groundLayer;

    [Tooltip("la taille du 'rayon' raycast si on veut sauter à nouveau sans toucher le sol")]
    public float groundDistance = 1.1f;

    public Rigidbody2D playerRigidbody = null; //pour bouger

    private Animator _animator = null; //pour mettre les anim

    private SpriteRenderer _spriteRenderer = null; // pour montrer le chara sur l'écran;  = null c'est pour etre sur que en gros ça fait ref à rien dès le départ

    public bool canMove = true;

    public bool isGrounded = false; //booléen pour savoir si je touche le sol ou pas    


    private void Awake()
    {
        //Void awake pour être lancé avant un quelconque start => ici pour choper les coponents direct et forcer le fait qu'ils soient sur l'objet où le script est mis
        playerRigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

    }


    private void Update()
    //à chaque frame on va appeler les 4 fonctions en dessous là
    {
        if (canMove) // boucle comme ça parce que si je veux bloquer les mouvements du player sans bloquer la physique plus tard 
        {
            UpdateMovement(); //met à jour le sens de l'anim 
            UpdateJump(); //vérifie si on appuie espace pour sauter
        }

        ClampVelocity(); //capte la vitesse pour limiter la vitesse max
        UpdateGround(); //vérifie si le joueur est bien au sol

        //ces lignes c'est dédié à l'aimation et à laquelle jouer selon la vitesse et l'état du perso
        ;
        // Quand la vitesse en X est négative, on inverse l'animation pour regarder à gauche
        if (Mathf.Abs(playerRigidbody.velocity.x) > 0.1f)
        {
            _spriteRenderer.flipX = playerRigidbody.velocity.x < 0;
        }
        // flip
        //mais tout ça dans le cas où à la base il était vers la droite, puisqu'on se base sur la vélocité

        _animator.SetFloat("xVelocity", Mathf.Abs(playerRigidbody.velocity.x));
        _animator.SetBool("isGrounded", isGrounded);
        _animator.SetFloat("yVelocity", playerRigidbody.velocity.y);
        _animator.SetBool("IsMoving", Mathf.Abs(playerRigidbody.velocity.x) > 0.1f);
    }


    private void UpdateMovement()
    {
        float xMovement = 0f; //initiatlise le mvt du perso à 0 sur x

        if (Input.GetKey(KeyCode.RightArrow))
        {
            xMovement += 1; //si fleche droite pressée -> +1 sur l'axe X donc avance vers droite
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            xMovement += -1; //si fleche gauche pressée -> -1 sur l'axe X donc avance vers gauche
        }

        if (xMovement == 0) //si perso bouge pas
        {
            playerRigidbody.velocity = new Vector2(Mathf.Lerp(playerRigidbody.velocity.x, 0, Time.deltaTime * 10f), playerRigidbody.velocity.y);

            // on met son mvt sur x à 0 mais on laisse son état sur y comme il est (donc s'il saute ou tombe)
        }
        else //se joue ds tous les cas dès qu'il y a un mvt (lorsque xmovement =/ 0) donc ds tous si une key est pressée
        {
            playerRigidbody.velocity += Vector2.right * xMovement * movementAcceleration * Time.deltaTime;
            //ok donc ici, il s'agit d'appliquer la vitesse selon la physique (la direction et le mvt a déjà été défini plus haut)
            //velocity c'est un composant de rigidbody qui représente la vitesse (sur x et y) et donc ici on va y ajouter un mvt 
            //vector2 c'est un vecteur 2d auquel on peut donner un x et y entre -1 et 1 (x:y) mais en ajoutant .right on lock le y à 0 et le x à 1
            //xmovement peut stocker 1 ou -1 donc c'est pour inverser ou non le résultat en négatif si vers la gauche car on a bloqué x en postitif en ajouter right à vector2
            //movement acceleration c'est la variable qu'on a déterminé tout en haut (donc on peut direct écrire 20f en théorie dans la fonction) et ici c'est la vitessse d'accélération donc
            //Time.deltaTime notre meilleur pote en gros c'est le temps entre 2 frames (ici aussi en théorie on pourrait une valeur à la place, sauf qu'en mettant la fonction instead, ça s'adapt au nb de fps)

        }

    }

    private void UpdateGround()
    {

        BoxCollider2D collider = GetComponent<BoxCollider2D>(); // On récupère la boîte
        Vector2 origin = new Vector2(collider.bounds.center.x, collider.bounds.min.y); // Point de départ en bas du collider

        RaycastHit2D hit = Physics2D.CircleCast(origin, 0.2f, Vector2.down, groundDistance, groundLayer);
        //Raycast envoie un rayon pour capter si y'a un collider en partant du bas de l'objet auquel on a assigné le script

        if (hit.collider != null) //si pas null alors a touché qqc, donc est au sol, donc je peux sauter
        {
            isGrounded = true; //touche collider = grounded
        }
        else
        {
            isGrounded = false; //touche pas collider = pas grounded
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Vector2 origin = new Vector2(collider.bounds.center.x, collider.bounds.min.y);
            Gizmos.DrawWireSphere(origin + Vector2.down * groundDistance, 0.2f);
        }
    }

    private void UpdateJump()
    {

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)  //DOWN nécessaire contrairement au dépacement car sinon se joue en boucle et is grounded pour être sûr d'être au sol
        {
            playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //donc qd espace pressé : on prend l'objet, on lui applique une force (pas un mvt parce que c'est juste une impulsion sur y) 
            //vector2.up expliqué plus haut * la variable force définie en haut 
            //forcemode2d.impulse appplique direct en mode impulsion

            isGrounded = false;
            //on remet à l'état faux jusqu'à ce que ça capte le sol et revien en true
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded == false)
        {
            playerRigidbody.AddForce(Vector2.down * fallForce, ForceMode2D.Impulse);
        }


    }

    private void ClampVelocity() //pour vérifier qu'on dépasse pas la vitesse max sur x dont on a définit la variable tt en haut 
    {
        Vector2 velocity = playerRigidbody.velocity; //on chope la valeur de la vélocité (que le x) actuelle 

        velocity.x = Mathf.Clamp(velocity.x, -maxMovementSpeed, maxMovementSpeed);
        //peu importe cette valeur elle doit être égale à ... = entre les deux valeurs (le max ou son inverse si on va vers la gauche) et si c'est le cas on lui assigne le max

        playerRigidbody.velocity = velocity;
        //et pouf on lui applique cette valeur 
    }

    public void SetCinematicMode(bool active)
    {
        if (active)
        {
            canMove = false;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.gravityScale = 0; // Désactive la gravité temporairement
            _animator.SetBool("IsMoving", false);
            _animator.SetBool("ForceIdle", true);
        }
        else
        {
            playerRigidbody.gravityScale = 1; // Rétabli la gravité
            _animator.SetBool("ForceIdle", false);
            canMove = true;
        }

    }

}