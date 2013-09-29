using UnityEngine;
using System.Collections;

public class MainPlane : MonoBehaviour {
	Texture2D img;

	float invWidth, invHeight;    // inverse of screen dimensions
	float aspectRatio, aspectRatio2;
	int counter = 0;
	MSAFluidSolver2D fluidSolver;
	bool drawFluid = true;
	Texture2D texture;
	Texture2D sourceTexture;
	RenderTexture myRenderTexture;

	Vector3 pVec;

	int WIDTH = 100;
	int HEIGHT = 100;
	float ddd = 0.0f;

	void Start () {
		// duplicate the original texture and assign to the material
		invWidth = 1.0f/WIDTH;
	    invHeight = 1.0f/HEIGHT;
	    aspectRatio = WIDTH * invHeight;
	    aspectRatio2 = aspectRatio * aspectRatio;

		fluidSolver = new MSAFluidSolver2D(128,128);
		fluidSolver.enableRGB(true).setFadeSpeed(0.003f).setDeltaT(0.5f).setVisc(0.0001f);
		//Debug.Log(fluidSolver.getWidth()+":::"+ fluidSolver.getHeight());
		
		texture = new Texture2D(fluidSolver.getWidth(),fluidSolver.getHeight());
		renderer.material.mainTexture = texture;

		sourceTexture = (Texture2D)Resources.Load("otoko130");//WH128+2=130
		Color[] cols = sourceTexture.GetPixels();//130*130
	
		for(var i = 0; i < fluidSolver.getNumCells(); i++){
			fluidSolver.rOld[i]  = cols[i].r;
			fluidSolver.gOld[i]  = cols[i].g;
	    	fluidSolver.bOld[i]  = cols[i].b;
		}
	}
	
    
    void OnMouseDrag() {   
    	RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out hit)){
			float mouseNormX = hit.point.x * invWidth;
	   		float mouseNormY = hit.point.y * invHeight;
	    	float mouseVelX = (hit.point.x - pVec.x) * invWidth;
	    	float mouseVelY = (hit.point.y - pVec.y) * invHeight;
	    	addForce(mouseNormX*10, mouseNormY*10, mouseVelX*10, mouseVelY*10);
        
			pVec = hit.point;

	     	Debug.Log(hit.point+"/"+mouseNormX+","+mouseNormY+" :: "+mouseVelX+","+mouseVelY);
        }
   }
    

	void Update () {
		counter++;
		fluidSolver.update();
        
		Color[] cols = texture.GetPixels();
		for( int i = 0; i < cols.Length; ++i ) {
			 int d = 2;			
   		    cols[i] = new Color(fluidSolver.r[i] * d, fluidSolver.g[i] * d, fluidSolver.b[i] * d);	
		}
		texture.SetPixels(cols);
		texture.Apply();
		//fastblur(imgFluid, 2);
	}

	void addForce(float x, float y, float dx, float dy) {
	    float speed = dx * dx  + dy * dy * aspectRatio2;    // balance the x and y components of speed with the screen aspect ratio
	    if(speed > 0) { 
	        float velocityMult = 30.0f;
			int index = fluidSolver.getIndexForNormalizedPosition(x, y);

	        fluidSolver.uOld[index] += dx * velocityMult;
	        fluidSolver.vOld[index] += dy * velocityMult;
		}
	}

}
