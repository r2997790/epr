# Push to GitHub

Your code is committed locally. To push to GitHub:

## 1. Create a new repository on GitHub

1. Go to [https://github.com/new](https://github.com/new)
2. Repository name: `EPR` (or your preferred name)
3. Choose **Public** or **Private**
4. **Do not** initialize with README, .gitignore, or license (you already have these)
5. Click **Create repository**

## 2. Add remote and push

Run these commands (replace `YOUR_USERNAME` with your GitHub username):

```powershell
cd "c:\Users\nayrs\OneDrive\Desktop\Projects\EPR"
git remote add origin https://github.com/YOUR_USERNAME/EPR.git
git branch -M main
git push -u origin main
```

Or if you use SSH:

```powershell
git remote add origin git@github.com:YOUR_USERNAME/EPR.git
git branch -M main
git push -u origin main
```

## 3. Connect Railway to GitHub (optional)

For automatic deployments on push:

1. Go to [Railway Dashboard](https://railway.com/project/b34f398b-f741-4ad5-8615-41a9c297aacf)
2. Select the **delightful-contentment** service
3. Go to **Settings** → **Source**
4. Connect your GitHub repo and select the branch to deploy from
